using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Animation
{
    abstract public class RCDurationController<CntrlType>
        : RCController<CntrlType> where CntrlType : RCSpatial
    {
        float _elaspedTime;
        float _duration;

        public RCDurationController()
            :base()
        {
            _elaspedTime = 0.0f;
            _duration = 0.0f;
        }

        protected void Begin(float duration)
        {
            if (!_isAnimating)
            {
                _duration = duration;
                _elaspedTime = 0.0f;
                _isAnimating = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isAnimating) return;

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
                FireAnimationComplete();
            }
        }

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