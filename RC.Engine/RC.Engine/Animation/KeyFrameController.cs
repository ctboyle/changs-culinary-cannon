using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework;

namespace RC.Engine.Animation
{
    /// <summary>
    /// Key frame controller for animations that perform translations,
    /// rotations, or scaling over a specific period of time
    /// </summary>
    /// <typeparam name="CntrlType">The actual <see cref="RCSpatial"/> type.</typeparam>
    public class RCKeyFrameController<CntrlType> 
        : DurationController<CntrlType> where CntrlType : RCSpatial
    {
        /// <summary>
        /// The type of interpolation that is to be performed.
        /// </summary>
        public enum InterpolationMode
        {
            Linear,
            SmoothStep
        }

        /// <summary>
        /// The type of quaternion interpolation that is to be performed.
        /// </summary>
        public enum QuaternionInterploationMode
        {
            Linear,
            Spherical
        }

        /// <summary>
        /// The starting translation vector.
        /// </summary>
        protected Vector3 _sourceTrans;
        
        /// <summary>
        /// The starting scale.
        /// </summary>
        protected Vector3 _sourceScale;

        /// <summary>
        /// The starting rotation.
        /// </summary>
        protected Quaternion _sourceRot;

        /// <summary>
        /// The ending translation vector.
        /// </summary>
        protected Vector3 _destTrans;

        /// <summary>
        /// The ending scale.
        /// </summary>
        protected Vector3 _destScale;

        /// <summary>
        /// The ending rotation.
        /// </summary>
        protected Quaternion _destRot;

        private bool _doTranslation;
        private bool _doScale;
        private bool _doRotation;
        private InterpolationMode _scaleMode;
        private InterpolationMode _translationMode;
        private InterpolationMode _rotationMode;
        private QuaternionInterploationMode _rotationCoordMode;

        /// <summary>
        /// The current scale interpolation mode for the controller.
        /// </summary>
        public InterpolationMode ScaleMode
        {
            get { return _scaleMode; }
            set { _scaleMode = value; }
        }

        /// <summary>
        /// The current translation interpolation mode for the controller.
        /// </summary>
        public InterpolationMode TranslationMode
        {
            get { return _translationMode; }
            set { _translationMode = value; }
        }

        /// <summary>
        /// The current rotation interpolation mode for the controller.
        /// </summary>
        public InterpolationMode RotationMode
        {
            get { return _rotationMode; }
            set { _rotationMode = value; }
        }

        /// <summary>
        /// The current rotation interpolation mode for the controller.
        /// </summary>
        public QuaternionInterploationMode RotationCoordinateMode
        {
            get { return _rotationCoordMode; }
            set { _rotationCoordMode = value; }
        }

        /// <summary>
        /// Flag to determine if translation is to be performed.
        /// </summary>
        public bool DoTranslation
        {
            get { return _doTranslation; }
            set { _doTranslation = value; }
        }

        /// <summary>
        /// Flag to determine if scaling is to be performed.
        /// </summary>
        public bool DoScale
        {
            get { return _doScale; }
            set { _doScale = value; }
        }

        /// <summary>
        /// Flag to determine if rotation is to be performed.
        /// </summary>
        public bool DoRotation
        {
            get { return _doRotation; }
            set { _doRotation = value; }
        }

        /// <summary>
        /// Creates a new instance of the key frame controller.
        /// </summary>
        public RCKeyFrameController()
            : base()
        {
            _doTranslation = true;
            _doScale = true;
            _doRotation = true;

            RotationMode = InterpolationMode.Linear;
            ScaleMode = InterpolationMode.Linear;
            TranslationMode = InterpolationMode.Linear;

            RotationCoordinateMode = QuaternionInterploationMode.Linear;
        }

        /// <summary>
        /// Starts a new animation.
        /// </summary>
        /// <param name="source">The source transformation.</param>
        /// <param name="destination">The resulting transformation.</param>
        /// <param name="duration">The duration.</param>
        public void BeginAnimation(
            Matrix source,
            Matrix destination,
            float duration
            )
        {
            Begin(duration);
            
            source.Decompose(
                out _sourceScale,
                out _sourceRot,
                out _sourceTrans
                );

            destination.Decompose(
                out _destScale,
                out _destRot,
                out _destTrans
                );
        }

        /// <summary>
        /// Updates the animation given a percentage complete.
        /// </summary>
        /// <param name="percentComplete">The percentage the animation is complete.</param>
        /// <param name="isLastFrame">If the animation is to be completed.</param>
        protected override void UpdateDurationAnimation(
            float percentComplete,
            bool isLastFrame
            )
        {
            Vector3 lerpTrans;
            Vector3 lerpScale;
            Quaternion lerpRot;

            _controlledItem.LocalTrans.Decompose(
                out lerpScale,
                out lerpRot,
                out lerpTrans
                );
           
            
            if (_doRotation)
            {
                float alteredPecentComplete = 0.0f;
                switch (RotationMode)
                {
                    case InterpolationMode.Linear:
                        alteredPecentComplete = percentComplete;
                        break;
                    case InterpolationMode.SmoothStep:
                        // Alter the percent complete to make smooth stepping.
                        alteredPecentComplete = MathHelper.SmoothStep(0.0f, 1.0f, percentComplete);
                        break;
                }

                switch (RotationCoordinateMode)
                {
                    case QuaternionInterploationMode.Linear:
                        lerpRot = Quaternion.Lerp(_sourceRot, _destRot, alteredPecentComplete);
                        break;
                    case QuaternionInterploationMode.Spherical:
                        lerpRot = Quaternion.Slerp(_sourceRot, _destRot, alteredPecentComplete);
                        break;

                }
                
            }
            if (_doScale)
            {
                switch (ScaleMode)
                {
                    case InterpolationMode.Linear:
                        lerpScale = Vector3.Lerp(_sourceScale, _destScale, percentComplete);
                        break;
                    case InterpolationMode.SmoothStep:
                        lerpScale = Vector3.SmoothStep(_sourceScale, _destScale, percentComplete);
                        break;
                }
                
            }

            if ( _doTranslation)
            {
                switch (TranslationMode)
                {
                    case InterpolationMode.Linear:
                        lerpTrans = Vector3.Lerp(_sourceTrans, _destTrans, percentComplete);
                        break;
                    case InterpolationMode.SmoothStep:
                        lerpTrans = Vector3.SmoothStep(_sourceTrans, _destTrans, percentComplete);
                        break;
                }
            }

            _controlledItem.LocalTrans =
                Matrix.CreateScale(lerpScale) *
                Matrix.CreateFromQuaternion(lerpRot) *
                Matrix.CreateTranslation(lerpTrans);
                
        }   
    }
}
