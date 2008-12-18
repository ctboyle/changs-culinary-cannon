using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Animation
{
    /// <summary>
    /// A duration controller that provides functionality for animations
    /// that last a certain amount of time.
    /// </summary>
    /// <typeparam name="CntrlType">The actual <see cref="RCSpatial"/> type.</typeparam>
    public abstract class DurationController<CntrlType>
        : Controller<CntrlType> where CntrlType : RCSpatial
    {
        /// <summary>
        /// Delegate definition for a AnimationComplete event.
        /// </summary>
        public delegate void AnimationCompleteHandler();

        /// <summary>
        /// Event occurs when an animation is complete.
        /// </summary>
        public event AnimationCompleteHandler OnComplete;

        private float _elaspedTime;
        private float _duration;

        /// <summary>
        /// Creates an instance of the duration controller.
        /// </summary>
        public DurationController()
            : base()
        {
            _elaspedTime = 0.0f;
            _duration = 0.0f;
        }

        /// <summary>
        /// Starts a new animation of the specified duration.
        /// </summary>
        /// <param name="duration">The duration of the animiation.</param>
        protected void Begin(float duration)
        {
            if (!_isAnimating)
            {
                _duration = duration;
                _elaspedTime = 0.0f;
                _isAnimating = true;
            }
        }

        /// <summary>
        /// Called during an update pass.  Updates the current iteration
        /// of the animation.
        /// </summary>
        /// <param name="gameTime">The current gametime.</param>
        public override void Update(GameTime gameTime)
        {
            if (_isAnimating)
            {
                bool isLastframe = false;
                float incrementTime = (float)gameTime.ElapsedRealTime.TotalSeconds;

                if (incrementTime > 0.03f)
                {
                    incrementTime = 0.03f;
                }

                _elaspedTime += incrementTime;

                if (_elaspedTime >= _duration)
                {
                    _isAnimating = false;
                    isLastframe = true;
                }

                UpdateDurationAnimation(PercentComplete, !_isAnimating);

                if (isLastframe)
                {
                    if (OnComplete != null)
                    {
                        OnComplete();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the animation given a percentage complete.
        /// </summary>
        /// <param name="percentComplete">The percentage the animation is complete.</param>
        /// <param name="isLastFrame">If the animation is to be completed.</param>
        protected abstract void UpdateDurationAnimation(
            float percentComplete,
            bool isLastFrame
            );
        
        /// <summary>
        /// Gets the precent complete of the last animation. [0.0f - 1.0f]
        /// </summary>
        public float PercentComplete
        {
            get 
            { 
                return MathHelper.Clamp(
                    _elaspedTime / _duration,
                    0.0f,
                    1.0f                    
                    ); 
            }
        }
    }
}
