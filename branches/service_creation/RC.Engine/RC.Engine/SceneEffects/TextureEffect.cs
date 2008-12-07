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

        public RCTextureEffect(RCContent<Texture2D> texture) 
            : base()
        {
            _texture = texture;
        }

        public RCTextureEffect(IRCContentRequester contentRqst, RCContent<Texture2D> texture)
            : base(contentRqst)
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

        public override object CreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            return content.Load<Effect>(EffectPath);
        }
    }
}