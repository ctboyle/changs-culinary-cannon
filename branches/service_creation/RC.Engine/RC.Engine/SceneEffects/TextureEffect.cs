using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;

namespace RC.Engine.SceneEffects
{
    public class RCTextureEffect : RCEffect
    {
        public const string EffectPath = "Content\\ShaderEffects\\Texture";

        private RCContent<Texture2D> _texture = null;

        public static RCTextureEffect Create(IRCContentRequester content, RCContent<Texture2D> texture)
        {
            return (RCTextureEffect)content.RequestContent<Effect>(
                delegate(Guid id, IRCContentManager c) 
                { 
                    return new RCTextureEffect(id, c, texture); 
                },
                RCTextureEffect.EffectPath
            );
        }

        public RCTextureEffect(
            Guid id, 
            IRCContentManager contentMgr, 
            RCContent<Texture2D> texture
            ) : base(id, contentMgr)
        {
            _texture = texture;
        }

        public override void CustomConfigure(IRCRenderManager render)
        {
            if (_texture != null)
            {
                Content.Parameters["xTexture"].SetValue(_texture.Content);
                Content.Parameters["xWorldViewProjection"].SetValue(render.World * render.View * render.Projection);
            }
        }
    }
}