using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Base;

namespace RC.Engine.Test.Particle
{
    class SmokePlumeParticleSystem : ParticleSystem
    {
        public SmokePlumeParticleSystem(IGraphicsDeviceService graphics, ParticleEffect effect)
            : base(graphics, effect)
        {
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.texture = new RCDefaultContent<Texture2D>(@"Content\smoke");

            settings.MaxParticles = 600;

            settings.Duration = TimeSpan.FromSeconds(10);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 5;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 5;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(-20, -5, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 5;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 50;
            settings.MaxEndSize = 200;
        }
    }
}
