using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Base;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;

namespace RC.Engine.Test.Particle
{
    class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public ExplosionSmokeParticleSystem(IGraphicsDeviceService graphics, ParticleEffect effect)
            : base(graphics, effect)
        {
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.texture = new RCDefaultContent<Texture2D>(@"Content\explosion");

            settings.MaxParticles = 15;

            settings.Duration = TimeSpan.FromSeconds(1);
            settings.DurationRandomness = 0;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 2;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 2;

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.Gray;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 2;
            settings.MaxEndSize = 2;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
