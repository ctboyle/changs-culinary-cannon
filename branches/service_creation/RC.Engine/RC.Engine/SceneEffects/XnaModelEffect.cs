using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;

namespace RC.Engine.SceneEffects
{
    public class RCModelPartEffect : RCEffect
    {
        public delegate void EffectConfigure(IRCRenderManager render);

        private EffectConfigure _ConfigureFn;
        protected Effect _effect;

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
        public RCModelPartEffect(Effect xnaEffect, IRCContentRequester contentRequest)
            : base(contentRequest)
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
