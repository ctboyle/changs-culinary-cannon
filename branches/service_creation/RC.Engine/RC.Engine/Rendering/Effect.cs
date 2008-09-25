using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RC.Engine.Rendering
{

    public abstract class RCEffect
    {
        private Effect myEffect;

        // Blending states for each pass.
        private RCAlphaState[] _alphaStates;

        private int _iPassQuantity;

        public Effect Effect
        {
            get
            {
                return myEffect;
            }
        }

        public string TechniqueName
        {
            get { return myEffect.CurrentTechnique.Name; }
            set
            {
                foreach (EffectTechnique technique in myEffect.Techniques)
                {
                    if (technique.Name == value)
                    {
                        myEffect.CurrentTechnique = myEffect.Techniques[value];

                        UpdateTechniqueInfo();
                        break;      
                    }
                }

                // Do not fail, if technique is not found.
            }
        }

        public RCEffect()
        {

        }

        private void UpdateTechniqueInfo()
        {
            if (myEffect.CurrentTechnique != null)
            {
                SetPassQuantity(myEffect.CurrentTechnique.Passes.Count);
            }
        }

        private void SetPassQuantity(int iPassQuantity)
        {
            _iPassQuantity = iPassQuantity;

            _alphaStates = new RCAlphaState[iPassQuantity];
            SetDefaultAlphaState();
        }

        private void SetDefaultAlphaState()
        {
            _alphaStates[0] = new RCAlphaState();
            _alphaStates[0].BlendEnabled = true;
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

                RCAlphaState savedState = (RCAlphaState)render.GetRenderState(RCRenderState.StateType.ALPHA);
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
                RCAlphaState savedState = (RCAlphaState)render.GetRenderState(RCRenderState.StateType.ALPHA);
                render.SetRenderState(_alphaStates[iPass]);
                _alphaStates[iPass] = savedState;
            }
        }
        



        protected abstract Effect LoadEffect(GraphicsDevice myDevice, ContentManager myLoader);

        public void LoadContent(GraphicsDevice myDevice, ContentManager myLoader)
        {
            myEffect = LoadEffect(myDevice, myLoader);
            UpdateTechniqueInfo();
        }

    }
    
}
