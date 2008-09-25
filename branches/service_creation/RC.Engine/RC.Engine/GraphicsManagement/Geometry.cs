using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using RC.Engine.SceneEffects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RC.Engine.GraphicsManagement
{
    public class RCGeometry : RCSpatial
    {
        public RCRenderStateCollection RenderStates = new RCRenderStateCollection(false);
        private RCLightEffect _lightEffect = null;

        private short[] _indices;

        // TODO: Need own index buffer.
        private IndexBuffer _iBuffer;
        
        private RCVertexBuffer _vBuffer;

        public IndexBuffer IBuffer
        {
            get { return _iBuffer; }
        }
        
        public RCVertexBuffer VBuffer
        {
            get { return _vBuffer; }
        }
        


        public RCGeometry(short[] indices, RCVertexBuffer vertexBuffer)
        {
            _indices = indices;
            _vBuffer = vertexBuffer;
        }

        public override void Load(GraphicsDevice device, ContentManager content)
        {
            base.Load(device, content);

            _iBuffer = new IndexBuffer(device, typeof(short), _indices.Length, BufferUsage.None);
            _iBuffer.SetData<short>(_indices);
            
            VBuffer.Load(device);
        }

        public override void Unload()
        {
            VBuffer.UnLoad();
            _indices = null;
        }
        

        public override void Draw(IRCRenderManager render)
        {
            render.Draw(this);
        }

        protected override void UpdateState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            foreach (RCRenderState.StateType type in Enum.GetValues(typeof(RCRenderState.StateType)))
            {
                RenderStates.Add(stateStack[type]);
            }

            // If lights, add lighting effect and update them.
            if (lightStack.Count > 0)
            {
                if (_lightEffect != null)
                {
                    _lightEffect.RemoveAllLights();
                }
                else
                {
                    // Create a new light effect and put it to be rendered first in the list.
                    _lightEffect = new RCLightEffect();
                    Effects.Insert(0, _lightEffect);
                }
                // Make sure the

                foreach (RCLight light in lightStack)
                {
                    _lightEffect.AddLight(light);
                }
            }
            else
            {
                if (_lightEffect != null)
                {
                    // No lights, remove it from the list.
                    Effects.Remove(_lightEffect);
                    _lightEffect = null; // TODO: See about keeping the refrence to avoid GC
                }
            }
        }

        protected override void UpdateWorldBound()
        {
            
        }
       
    }
}
