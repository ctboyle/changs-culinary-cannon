using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using RC.Engine.ContentManagement;


namespace RC.Engine.Particle
{
    public abstract class EmitterParticleSystem : ParticleSystem
    {
        public EmitterParticleSystem(IGraphicsDeviceService graphics, ParticleEffect effect)
            : base(graphics, effect)
        {
            ParticleEmitterController controller = CreateParticleEmiterController();

            controller.AttachToObject(this);
        }

        protected abstract ParticleEmitterController CreateParticleEmiterController();
    }
}
