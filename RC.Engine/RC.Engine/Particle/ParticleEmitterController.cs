using System;
using System.Collections.Generic;
using System.Text;

using RC.Engine.Animation;

using Microsoft.Xna.Framework;

namespace RC.Engine.Particle
{
    public class ParticleEmitterController : Controller<ParticleSystem>
    {
        bool resetPosition;
        float timeBetweenParticles;
        Vector3 newPosition;
        Vector3 previousPosition;
        float timeLeftOver;


        /// <summary>
        /// Tells the emitter to reset the previous position.
        /// 
        /// Use this when the particle system position has 
        /// instantaneously moved.
        /// 
        /// Auto resets to false after next update.
        /// </summary>
        public bool ResetPosition
        {
            get { return resetPosition; }
            set { resetPosition = value; }
        }

        public ParticleEmitterController(float particlesPerSecond)
        {
            timeBetweenParticles = 1.0f / particlesPerSecond;
        }

        protected override void OnAttach()
        {
            // This item has just been attached, ensure a new
            // initial position is taken.
            resetPosition = true;

            base.OnAttach();
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // Update the new position;
            newPosition = _controlledItem.WorldTrans.Translation;

            // Set the inital position if needed.
            if (resetPosition)
            {
                previousPosition = newPosition;
                resetPosition = false;
            }

            // Work out how much time has passed since the previous update.
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                // If we had any time left over that we didn't use during the
                // previous update, add that to the current elapsed time.
                float timeToSpend = timeLeftOver + elapsedTime;

                // Counter for looping over the time interval.
                float currentTime = -timeLeftOver;

                // Create particles as long as we have a big enough time interval.
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    Vector3 position = CalculateParticlePosition(elapsedTime, currentTime);

                    Vector3 velocity = CalculateParticleVelocity(elapsedTime, currentTime);

                    // Create the particle.
                    _controlledItem.AddParticle(position, velocity);
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }

            previousPosition = newPosition;
        }

        protected virtual Vector3 CalculateParticleVelocity(float elapsedTime, float currentTime)
        {
            // Work out how fast we are moving.
            return (newPosition - previousPosition) / elapsedTime;
        }

        protected virtual Vector3 CalculateParticlePosition(float elapsedTime, float currentTime)
        {
            // Work out the optimal position for this particle. This will produce
            // evenly spaced particles regardless of the object speed, particle
            // creation frequency, or game update rate.
            float mu = currentTime / elapsedTime;

            return Vector3.Lerp(previousPosition, newPosition, mu);
        }
    }
}
