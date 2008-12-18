using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;
using Microsoft.Xna.Framework;

namespace RC.Engine.SceneEffects
{
    public class RCModelPartEffect : RCEffect
    {
        public delegate void EffectConfigure(IRCRenderManager render);

        private EffectConfigure _ConfigureFn;
        protected Effect _effect;

        RCMaterialState _materialState = new RCMaterialState();

        public override void SetRenderState(int iPass, IRCRenderManager render, bool isPrimaryEffect)
        {
            if (Content is BasicEffect)
            {
                BasicEffect modelEffect = (BasicEffect)Content;


                RCMaterialState savedState = (RCMaterialState)render.GetRenderState(RCRenderState.StateType.Material);
                render.SetRenderState(_materialState);
                _materialState = savedState;



            }

            base.SetRenderState(iPass, render, isPrimaryEffect);
        }

        public override void RestoreRenderState(int iPass, IRCRenderManager render, bool isPrimaryEffect)
        {
            if (Content is BasicEffect)
            {
                BasicEffect modelEffect = (BasicEffect)Content;


                RCMaterialState savedState = (RCMaterialState)render.GetRenderState(RCRenderState.StateType.Material);
                render.SetRenderState(_materialState);
                _materialState = savedState;
            }
            base.RestoreRenderState(iPass, render, isPrimaryEffect);
        }

        /// <summary>
        /// 
        /// </summary>
        public EffectConfigure EffectConfigurationFn
        {
            set { _ConfigureFn = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xnaEffect"></param>
        public RCModelPartEffect(Effect xnaEffect)
            : base()
        {
            if (xnaEffect == null)
            {
                throw new ArgumentNullException();
            }
            
            _effect = xnaEffect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="render"></param>
        public override void CustomConfigure(IRCRenderManager render)
        {
            if (_ConfigureFn != null)
            {
                _ConfigureFn(render);
            }

            if (Content is BasicEffect)
            {
                BasicEffect modelEffect = (BasicEffect)Content;              

                modelEffect.World = render.World;
                modelEffect.View = render.View;
                modelEffect.Projection = render.Projection;


                _materialState.Ambient =  new Color(modelEffect.AmbientLightColor);
                _materialState.Diffuse = new Color(modelEffect.DiffuseColor);
                _materialState.Specular = new Color(modelEffect.SpecularColor);
                _materialState.Emissive = new Color(modelEffect.EmissiveColor);
                _materialState.Shininess = modelEffect.SpecularPower;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected override object OnCreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            return _effect;
        }
    }
}
