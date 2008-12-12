using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.GraphicsManagement;
using RC.Engine.Animation;
using Microsoft.Xna.Framework;
using RC.Engine.Base;

namespace RC.Engine.Test.Particle
{
    public abstract class ParticleSystem : RCSpatial
    {
        #region Fields

        // Settings class controls the appearance and animation of this particle system.
        public ParticleSettings Settings = new ParticleSettings();

        // Custom effect for drawing point sprite particles. This computes the particle
        // animation entirely in the vertex shader: no per-particle CPU work required!
        public ParticleEffect Effect = null;

        // An array of particles, treated as a circular queue.
        public ParticleVertex[] Particles;

        // A vertex buffer holding our particles. This contains the same data as
        // the particles array, but copied across to where the GPU can access it.
        public DynamicVertexBuffer VertexBuffer;

        // Vertex declaration describes the format of our ParticleVertex structure.
        public VertexDeclaration VertexDeclaration;

        // The particles array and vertex buffer are treated as a circular queue.
        // Initially, the entire contents of the array are free, because no particles
        // are in use. When a new particle is created, this is allocated from the
        // beginning of the array. If more than one particle is created, these will
        // always be stored in a consecutive block of array elements. Because all
        // particles last for the same amount of time, old particles will always be
        // removed in order from the start of this active particle region, so the
        // active and free regions will never be intermingled. Because the queue is
        // circular, there can be times when the active particle region wraps from the
        // end of the array back to the start. The queue uses modulo arithmetic to
        // handle these cases. For instance with a four entry queue we could have:
        //
        //      0
        //      1 - first active particle
        //      2 
        //      3 - first free particle
        //
        // In this case, particles 1 and 2 are active, while 3 and 4 are free.
        // Using modulo arithmetic we could also have:
        //
        //      0
        //      1 - first free particle
        //      2 
        //      3 - first active particle
        //
        // Here, 3 and 0 are active, while 1 and 2 are free.
        //
        // But wait! The full story is even more complex.
        //
        // When we create a new particle, we add them to our managed particles array.
        // We also need to copy this new data into the GPU vertex buffer, but we don't
        // want to do that straight away, because setting new data into a vertex buffer
        // can be an expensive operation. If we are going to be adding several particles
        // in a single frame, it is faster to initially just store them in our managed
        // array, and then later upload them all to the GPU in one single call. So our
        // queue also needs a region for storing new particles that have been added to
        // the managed array but not yet uploaded to the vertex buffer.
        //
        // Another issue occurs when old particles are retired. The CPU and GPU run
        // asynchronously, so the GPU will often still be busy drawing the previous
        // frame while the CPU is working on the next frame. This can cause a
        // synchronization problem if an old particle is retired, and then immediately
        // overwritten by a new one, because the CPU might try to change the contents
        // of the vertex buffer while the GPU is still busy drawing the old data from
        // it. Normally the graphics driver will take care of this by waiting until
        // the GPU has finished drawing inside the VertexBuffer.SetData call, but we
        // don't want to waste time waiting around every time we try to add a new
        // particle! To avoid this delay, we can specify the SetDataOptions.NoOverwrite
        // flag when we write to the vertex buffer. This basically means "I promise I
        // will never try to overwrite any data that the GPU might still be using, so
        // you can just go ahead and update the buffer straight away". To keep this
        // promise, we must avoid reusing vertices immediately after they are drawn.
        //
        // So in total, our queue contains four different regions:
        //
        // Vertices between firstActiveParticle and firstNewParticle are actively
        // being drawn, and exist in both the managed particles array and the GPU
        // vertex buffer.
        //
        // Vertices between firstNewParticle and firstFreeParticle are newly created,
        // and exist only in the managed particles array. These need to be uploaded
        // to the GPU at the start of the next draw call.
        //
        // Vertices between firstFreeParticle and firstRetiredParticle are free and
        // waiting to be allocated.
        //
        // Vertices between firstRetiredParticle and firstActiveParticle are no longer
        // being drawn, but were drawn recently enough that the GPU could still be
        // using them. These need to be kept around for a few more frames before they
        // can be reallocated.

        public float CurrentTime;

        public int FirstActiveParticle;
        public int FirstNewParticle;
        public int FirstFreeParticle;
        public int FirstRetiredParticle;

        // Count how many times Draw has been called. This is used to know
        // when it is safe to retire old particles back into the free list.
        public int DrawCounter;

        // Shared random number generator.
        static Random Random = new Random();


        #endregion

        public ParticleSystem(RCGameContext ctx, ParticleEffect effect)
        {
            VertexDeclaration = new VertexDeclaration(ctx.Graphics.GraphicsDevice,
                                                      ParticleVertex.VertexElements);

            Particles = new ParticleVertex[Settings.MaxParticles];

            // Create a dynamic vertex buffer.
            int size = ParticleVertex.SizeInBytes * Particles.Length;
            VertexBuffer = new DynamicVertexBuffer(ctx.Graphics.GraphicsDevice, size,
                                                   BufferUsage.WriteOnly |
                                                   BufferUsage.Points);
            Effect = effect;

            Effect.Settings = this.Settings;

            InitializeSettings(ctx, Settings);

            Controller<ParticleSystem> controller = new ParticleController();
            controller.AttachToObject(this);
        }

        /// <summary>
        /// Derived particle system classes should override this method
        /// and use it to initalize their tweakable settings.
        /// </summary>
        protected abstract void InitializeSettings(RCGameContext ctx, ParticleSettings settings);


        #region Utility

        /// <summary>
        /// Adds a new particle to the system.
        /// </summary>
        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = FirstFreeParticle + 1;

            if (nextFreeParticle >= Particles.Length)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == FirstRetiredParticle)
                return;

            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            velocity *= Settings.EmitterVelocitySensitivity;

            // Add in some random amount of horizontal velocity.
            float horizontalVelocity = MathHelper.Lerp(Settings.MinHorizontalVelocity,
                                                       Settings.MaxHorizontalVelocity,
                                                       (float)Random.NextDouble());

            double horizontalAngle = Random.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            // Add in some random amount of vertical velocity.
            velocity.Y += MathHelper.Lerp(Settings.MinVerticalVelocity,
                                          Settings.MaxVerticalVelocity,
                                          (float)Random.NextDouble());

            // Choose four random control values. These will be used by the vertex
            // shader to give each particle a different size, rotation, and color.
            Color randomValues = new Color((byte)Random.Next(255),
                                           (byte)Random.Next(255),
                                           (byte)Random.Next(255),
                                           (byte)Random.Next(255));

            // Fill in the particle vertex structure.
            Particles[FirstFreeParticle].Position = position;
            Particles[FirstFreeParticle].Velocity = velocity;
            Particles[FirstFreeParticle].Random = randomValues;
            Particles[FirstFreeParticle].Time = CurrentTime;

            FirstFreeParticle = nextFreeParticle;
        }

        #endregion

        #region Draw
        public override void Draw(RC.Engine.Rendering.IRCRenderManager render, RC.Engine.ContentManagement.IRCContentRequester contentRqst)
        {
            GraphicsDevice device = render.Graphics.GraphicsDevice;

            // Restore the vertex buffer contents if the graphics device was lost.
            if (VertexBuffer.IsContentLost)
            {
                VertexBuffer.SetData(Particles);
            }

            // If there are any particles waiting in the newly added queue,
            // we'd better upload them to the GPU ready for drawing.
            if (FirstNewParticle != FirstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            // If there are any active particles, draw them now!
            if (FirstActiveParticle != FirstFreeParticle)
            {
                Effect.CustomConfigure(render);

                SetParticleRenderStates(device.RenderState);

                Effect.effectViewParameter.SetValue(render.View);
                Effect.effectProjectionParameter.SetValue(render.Projection);

                // Set an effect parameter describing the viewport size. This is needed
                // to convert particle sizes into screen space point sprite sizes.
                Effect.effectViewportHeightParameter.SetValue(device.Viewport.Height);

                // Set an effect parameter describing the current time. All the vertex
                // shader particle animation is keyed off this value.
                Effect.effectTimeParameter.SetValue(CurrentTime);

                // Set the particle vertex buffer and vertex declaration.
                device.Vertices[0].SetSource(VertexBuffer, 0,
                                             ParticleVertex.SizeInBytes);

                device.VertexDeclaration = VertexDeclaration;

                // Activate the particle effect.
                Effect.Content.Begin();

                foreach (EffectPass pass in Effect.Content.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    if (FirstActiveParticle < FirstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        device.DrawPrimitives(PrimitiveType.PointList,
                                              FirstActiveParticle,
                                              FirstFreeParticle - FirstActiveParticle);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        device.DrawPrimitives(PrimitiveType.PointList,
                                              FirstActiveParticle,
                                              Particles.Length - FirstActiveParticle);

                        if (FirstFreeParticle > 0)
                        {
                            device.DrawPrimitives(PrimitiveType.PointList,
                                                  0,
                                                  FirstFreeParticle);
                        }
                    }

                    pass.End();
                }

                Effect.Content.End();

                // Reset a couple of the more unusual renderstates that we changed,
                // so as not to mess up any other subsequent drawing.
                device.RenderState.PointSpriteEnable = false;
                device.RenderState.DepthBufferWriteEnable = true;
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            }

            DrawCounter++;
        }

        /// <summary>
        /// Helper for uploading new particles from our managed
        /// array to the GPU vertex buffer.
        /// </summary>
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (FirstNewParticle < FirstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                VertexBuffer.SetData(FirstNewParticle * stride, Particles,
                                     FirstNewParticle,
                                     FirstFreeParticle - FirstNewParticle,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                VertexBuffer.SetData(FirstNewParticle * stride, Particles,
                                     FirstNewParticle,
                                     Particles.Length - FirstNewParticle,
                                     stride, SetDataOptions.NoOverwrite);

                if (FirstFreeParticle > 0)
                {
                    VertexBuffer.SetData(0, Particles,
                                         0, FirstFreeParticle,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            // Move the particles we just uploaded from the new to the active queue.
            FirstNewParticle = FirstFreeParticle;
        }


        /// <summary>
        /// Helper for setting the renderstates used to draw particles.
        /// </summary>
        void SetParticleRenderStates(RenderState renderState)
        {
            // Enable point sprites.
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;

            // Set the alpha blend mode.
            renderState.AlphaBlendEnable = true;
            renderState.AlphaBlendOperation = BlendFunction.Add;
            renderState.SourceBlend = Settings.SourceBlend;
            renderState.DestinationBlend = Settings.DestinationBlend;

            // Set the alpha test mode.
            renderState.AlphaTestEnable = true;
            renderState.AlphaFunction = CompareFunction.Greater;
            renderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;
        }
        #endregion

        protected override void UpdateWorldBound()
        {
        }

        protected override void UpdateState(RC.Engine.Rendering.RCRenderStateStack stateStack, Stack<RC.Engine.SceneEffects.RCLight> lightStack)
        {
        }
    }
}
