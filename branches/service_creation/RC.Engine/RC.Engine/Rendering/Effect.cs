using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;

namespace RC.Engine.Rendering
{
    /// <summary>
    /// Defines a wraper for shaders in RC Engine
    /// </summary>
    public abstract class RCEffect : RCContent<Effect>
    {
        // Blending states for each pass.
        private RCAlphaState[] _alphaStates;
        private int _iPassQuantity;

        /// <summary>
        /// Gets or sets the current technique for the effect.
        /// </summary>
        public string TechniqueName
        {
            get { return Content.CurrentTechnique.Name; }
            set
            {
                foreach (EffectTechnique technique in Content.Techniques)
                {
                    if (technique.Name == value)
                    {
                        Content.CurrentTechnique = Content.Techniques[value];
                        UpdateTechniqueInfo();
                        break;      
                    }
                }

                // Do not fail, if technique is not found.
            }
        }

        public RCEffect()
            : base()
        {
        }

        /// <summary>
        /// Updates pass quanity based on current technique.
        /// </summary>
        private void UpdateTechniqueInfo()
        {
            if (Content.CurrentTechnique != null)
            {
                SetPassQuantity(Content.CurrentTechnique.Passes.Count);
            }
        }

        /// <summary>
        /// Sets the number of passes to be used for the current effect
        /// </summary>
        /// <param name="iPassQuantity"></param>
        private void SetPassQuantity(int iPassQuantity)
        {
            _iPassQuantity = iPassQuantity;
            _alphaStates = new RCAlphaState[iPassQuantity];
            SetDefaultAlphaState();
        }

        /// <summary>
        /// Sets the defaults alpha states for each pass for the effect.
        /// </summary>
        protected virtual void SetDefaultAlphaState()
        {
            for (int i = 0; i < _iPassQuantity; i++)
            {
                _alphaStates[i] = new RCAlphaState();
                _alphaStates[i].BlendEnabled = true;
                _alphaStates[i].SrcBlend = Blend.DestinationColor;
                _alphaStates[i].DstBlend = Blend.Zero;
            }
        }

        /// <summary>
        /// Gets the alpha state associated with the specified pass.
        /// </summary>
        /// <param name="iPass">The pass to get alpha state information.</param>
        /// <returns></returns>
        public RCAlphaState GetBlending(int iPass)
        {
            return _alphaStates[iPass];
        }

        /// <summary>
        /// Overide to set custom effect properties needed for the effect.
        /// </summary>
        /// <param name="render"></param>
        public abstract void CustomConfigure(IRCRenderManager render);

        /// <summary>
        /// Sets the current render state to that of the pass.
        /// </summary>

        public virtual void SetRenderState(
            int iPass, 
            IRCRenderManager render,
            bool isPrimaryEffect)
        {
            if (!isPrimaryEffect || iPass > 0)
            {
                _alphaStates[iPass].BlendEnabled = true;

                RCAlphaState savedState = (RCAlphaState)render.GetRenderState(RCRenderState.StateType.Alpha);
                render.SetRenderState(_alphaStates[iPass]);
                _alphaStates[iPass] = savedState;
            }
        }

        /// <summary>
        /// Restores the previously set alpha states to what they were.
        /// </summary>
        /// <param name="iPass">The pass to get the alpha state.</param>
        /// <param name="render">The render manager</param>
        /// <param name="isPrimaryEffect">Indicates weather this is the first effect.</param>
        public virtual void RestoreRenderState(
            int iPass, 
            IRCRenderManager render,
            bool isPrimaryEffect)
        {
            if (!isPrimaryEffect || iPass > 0)
            {
                RCAlphaState savedState = (RCAlphaState)render.GetRenderState(RCRenderState.StateType.Alpha);
                render.SetRenderState(_alphaStates[iPass]);
                _alphaStates[iPass] = savedState;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void OnFinishedLoad()
        {
            UpdateTechniqueInfo();
            base.OnFinishedLoad();
        }

        protected override abstract object OnCreateType(IGraphicsDeviceService graphics, ContentManager content);
    } 
}