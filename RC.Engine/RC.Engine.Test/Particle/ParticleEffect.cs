using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;

namespace RC.Engine.Test.Particle
{
    public class ParticleEffect : RCEffect
    {
        public const string EffectName = @"Content\ParticleEffect";

        public ParticleSettings Settings;
        public EffectParameter effectViewParameter;
        public EffectParameter effectProjectionParameter;
        public EffectParameter effectViewportHeightParameter;
        public EffectParameter effectTimeParameter;

        public ParticleEffect(IRCContentRequester contReq)
            : base(contReq)
        {
        }

        public override void CustomConfigure(IRCRenderManager render)
        {
            EffectParameterCollection parameters = Content.Parameters;

            // Look up shortcuts for parameters that change every frame.
            effectViewParameter = parameters["View"];
            effectProjectionParameter = parameters["Projection"];
            effectViewportHeightParameter = parameters["ViewportHeight"];
            effectTimeParameter = parameters["CurrentTime"];

            // Set the values of parameters that do not change.
            parameters["Duration"].SetValue((float)Settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(Settings.DurationRandomness);
            parameters["Gravity"].SetValue(Settings.Gravity);
            parameters["EndVelocity"].SetValue(Settings.EndVelocity);
            parameters["MinColor"].SetValue(Settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(Settings.MaxColor.ToVector4());

            parameters["RotateSpeed"].SetValue(
                new Vector2(Settings.MinRotateSpeed, Settings.MaxRotateSpeed));

            parameters["StartSize"].SetValue(
                new Vector2(Settings.MinStartSize, Settings.MaxStartSize));

            parameters["EndSize"].SetValue(
                new Vector2(Settings.MinEndSize, Settings.MaxEndSize));

            // Load the particle texture, and set it onto the effect.
            parameters["Texture"].SetValue(Settings.texture.Content);

            // Choose the appropriate effect technique. If these particles will never
            // rotate, we can use a simpler pixel shader that requires less GPU power.
            string techniqueName;

            if ((Settings.MinRotateSpeed == 0) && (Settings.MaxRotateSpeed == 0))
                techniqueName = "NonRotatingParticles";
            else
                techniqueName = "RotatingParticles";

            Content.CurrentTechnique = Content.Techniques[techniqueName];
        }

        protected override object OnCreateType(Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService graphics, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Effect effect = content.Load<Effect>(EffectName);

            // If we have several particle systems, the content manager will return
            // a single shared effect instance to them all. But we want to preconfigure
            // the effect with parameters that are specific to this particular
            // particle system. By cloning the effect, we prevent one particle system
            // from stomping over the parameter settings of another.

            Effect particleEffect = effect.Clone(graphics.GraphicsDevice);

            return particleEffect;
        }
    }
}
