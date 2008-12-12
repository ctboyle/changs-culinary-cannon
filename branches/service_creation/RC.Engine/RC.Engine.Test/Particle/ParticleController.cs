using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Animation;

namespace RC.Engine.Test.Particle
{
    public class ParticleController : Controller<ParticleSystem>
    {
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (gameTime == null) return;

            _controlledItem.CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            RetireActiveParticles();
            FreeRetiredParticles();

            // If we let our timer go on increasing for ever, it would eventually
            // run out of floating point precision, at which point the particles
            // would render incorrectly. An easy way to prevent this is to notice
            // that the time value doesn't matter when no particles are being drawn,
            // so we can reset it back to zero any time the active queue is empty.

            if (_controlledItem.FirstActiveParticle == _controlledItem.FirstFreeParticle)
                _controlledItem.CurrentTime = 0;

            if (_controlledItem.FirstRetiredParticle == _controlledItem.FirstActiveParticle)
                _controlledItem.DrawCounter = 0;
        }

        /// <summary>
        /// Helper for checking when active particles have reached the end of
        /// their life. It moves old particles from the active area of the queue
        /// to the retired section.
        /// </summary>
        void RetireActiveParticles()
        {
            float particleDuration = (float)_controlledItem.Settings.Duration.TotalSeconds;

            while (_controlledItem.FirstActiveParticle != _controlledItem.FirstNewParticle)
            {
                // Is this particle old enough to retire?
                float particleAge = _controlledItem.CurrentTime - _controlledItem.Particles[_controlledItem.FirstActiveParticle].Time;

                if (particleAge < particleDuration)
                    break;

                // Remember the time at which we retired this particle.
                _controlledItem.Particles[_controlledItem.FirstActiveParticle].Time = _controlledItem.DrawCounter;

                // Move the particle from the active to the retired queue.
                _controlledItem.FirstActiveParticle++;

                if (_controlledItem.FirstActiveParticle >= _controlledItem.Particles.Length)
                    _controlledItem.FirstActiveParticle = 0;
            }
        }


        /// <summary>
        /// Helper for checking when retired particles have been kept around long
        /// enough that we can be sure the GPU is no longer using them. It moves
        /// old particles from the retired area of the queue to the free section.
        /// </summary>
        void FreeRetiredParticles()
        {
            while (_controlledItem.FirstRetiredParticle != _controlledItem.FirstActiveParticle)
            {
                // Has this particle been unused long enough that
                // the GPU is sure to be finished with it?
                int age = _controlledItem.DrawCounter - (int)_controlledItem.Particles[_controlledItem.FirstRetiredParticle].Time;

                // The GPU is never supposed to get more than 2 frames behind the CPU.
                // We add 1 to that, just to be safe in case of buggy drivers that
                // might bend the rules and let the GPU get further behind.
                if (age < 3)
                    break;

                // Move the particle from the retired to the free queue.
                _controlledItem.FirstRetiredParticle++;

                if (_controlledItem.FirstRetiredParticle >= _controlledItem.Particles.Length)
                    _controlledItem.FirstRetiredParticle = 0;
            }
        }
    }
}
