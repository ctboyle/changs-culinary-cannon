using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using RC.Content.Heightmap;
using RC.Engine.ContentManagement;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.SceneEffects
{
    public class HeightMapEffect : RCEffect
    {
        public const String EffectPath = "Content\\Effects\\Terrain";

        private RCContent<RCHeightMap> _heightMap = null;
        private RCContent<Texture2D> _grassTex = null;
        private RCContent<Texture2D> _rockTex = null;
        private RCContent<Texture2D> _snowTex = null;
        private float scaling = 1;

        public HeightMapEffect(
            IRCContentRequester contentRqst, 
            RCContent<RCHeightMap> heightMap,
            RCContent<Texture2D> grassTex,
            RCContent<Texture2D> rockTex,
            RCContent<Texture2D> snowTex
            ) : base(contentRqst)
        {
            _heightMap = heightMap;
            _grassTex = grassTex;
            _rockTex = rockTex;
            _snowTex = snowTex;
            scaling = heightMap.Content.Scaling;
        }

        public override void CustomConfigure(IRCRenderManager render)
        {
            if (_heightMap != null)
            {
                Content.Parameters["World"].SetValue(render.World);
                Content.Parameters["View"].SetValue(render.View);
                Content.Parameters["Projection"].SetValue(render.Projection);

                Content.Parameters["multiTextureBottom"].SetValue(_grassTex);
                Content.Parameters["multiTextureMiddle"].SetValue(_rockTex);
                Content.Parameters["multiTextureTop"].SetValue(_snowTex);
                Content.Parameters["bottomTextureUnblendedMax"].SetValue(.3f*scaling);
                Content.Parameters["middleTextureUnblendedMin"].SetValue(.35f*scaling);
                Content.Parameters["middleTextureUnblendedMax"].SetValue(.45f*scaling);
                Content.Parameters["topTextureUnblendedMin"].SetValue(.55f*scaling);
            }
        }

        protected override object OnCreateType(Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService graphics, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            return content.Load<Effect>(EffectPath);
        }
    }
}
