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
        private float totalHeightscaling = 1;
        private float bottomOfRockHeightPercent = .2f;
        private float topOfGrassHeightPercent = .35f;
        private float bottomOfSnowHeightPercent = .4f;
        private float topOfRockHeightPercent = .5f;

        public HeightMapEffect(
            IRCContentRequester contentRqst,
            RCContent<RCHeightMap> heightMap,
            RCContent<Texture2D> grassTex,
            RCContent<Texture2D> rockTex,
            RCContent<Texture2D> snowTex,
            float percentBottomOfRockHeight,
            float percentTopOfGrassHeight,
            float percentBottomOfSnowHeight,
            float percentTopOfRockHeight
            )
            : base(contentRqst)
        {
            _heightMap = heightMap;
            _grassTex = grassTex;
            _rockTex = rockTex;
            _snowTex = snowTex;
            totalHeightscaling = heightMap.Content.Scaling * heightMap.Content.HeightScaling;
            bottomOfRockHeightPercent = percentBottomOfRockHeight;
            topOfGrassHeightPercent = percentTopOfGrassHeight;
            bottomOfSnowHeightPercent = percentBottomOfSnowHeight;
            topOfRockHeightPercent = percentTopOfRockHeight;
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

                Content.Parameters["bottomTextureUnblendedMax"].SetValue(bottomOfRockHeightPercent
                    * totalHeightscaling);
                Content.Parameters["middleTextureUnblendedMin"].SetValue(topOfGrassHeightPercent
                    * totalHeightscaling);
                Content.Parameters["middleTextureUnblendedMax"].SetValue(bottomOfSnowHeightPercent
                    * totalHeightscaling);
                Content.Parameters["topTextureUnblendedMin"].SetValue(topOfRockHeightPercent
                    * totalHeightscaling);
            }
        }

        protected override object OnCreateType(Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService graphics, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            return content.Load<Effect>(EffectPath);
        }
    }
}
