using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RC.Engine.SceneEffects
{
    public class RCTextureEffect : RCEffect
    {
        private string _assetPath;
        private Texture2D _texture;
        
        public RCTextureEffect(string assetPath)
        {
            _assetPath = assetPath;
        }

        public override void CustomConfigure(IRCRenderManager render)
        {
            if (_texture != null)
            {
                Effect.Parameters["xTexture"].SetValue(_texture);
                Effect.Parameters["xWorldViewProjection"].SetValue(render.World * render.View * render.Projection);
            }
        }

        

        protected override Effect LoadEffect(GraphicsDevice myDevice, ContentManager myLoader)
        {

            _texture = myLoader.Load<Texture2D>(_assetPath);
            return myLoader.Load<Effect>("Content\\ShaderEffects\\Texture");
        }
    }
}
