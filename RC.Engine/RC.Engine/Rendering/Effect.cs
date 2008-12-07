using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;

namespace RC.Engine.Rendering
{
    public abstract class RCEffect : RCContent<Effect>
    {
        // Blending states for each pass.
        private RCAlphaState[] _alphaStates;
        private int _iPassQuantity;

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

        public RCEffect(IRCContentRequester contentRqst)
            : base(contentRqst)
        {
        }

        private void UpdateTechniqueInfo()
        {
            if (Content.CurrentTechnique != null)
            {
                SetPassQuantity(Content.CurrentTechnique.Passes.Count);
            }
        }

        private void SetPassQuantity(int iPassQuantity)
        {
            _iPassQuantity = iPassQuantity;
            _alphaStates = new RCAlphaState[iPassQuantity];
            SetDefaultAlphaState();
        }

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

        public RCAlphaState GetBlending(int iPass)
        {
            return _alphaStates[iPass];
        }

        public abstract void CustomConfigure(IRCRenderManager render);

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
            UpdateTechniqueInfo();
            base.OnInitialize();
        }

        public override abstract object CreateType(IGraphicsDeviceService graphics, ContentManager content);
    } 
}