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

        public static int NumIntervalsX = 100;
        public static int NumIntervalsY = 100;
        public static float SizeX = 200;
        public static float SizeY = 200;

        private RCContent<RCHeightMap> _heightMap = null;
        private RCContent<Texture2D> _grassTex = null;
        private RCContent<Texture2D> _rockTex = null;
        private RCContent<Texture2D> _snowTex = null;

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
        }

        //public HeightMapEffect(RCHeightMap heightMap)
        //{
        //    _heightMap = heightMap;
        //}

        public override void CustomConfigure(IRCRenderManager render)
        {
            if (_heightMap != null)
            {
                Content.Parameters["HeightMap"].SetValue(_heightMap.Content);
                Content.Parameters["World"].SetValue(render.World);
                Content.Parameters["View"].SetValue(render.View);
                Content.Parameters["Projection"].SetValue(render.Projection);

                Content.Parameters["multiTextureBottom"].SetValue(_grassTex);
                Content.Parameters["multiTextureMiddle"].SetValue(_rockTex);
                Content.Parameters["multiTextureTop"].SetValue(_snowTex);
                Content.Parameters["bottomTextureUnblendedMax"].SetValue(.3f);
                Content.Parameters["middleTextureUnblendedMin"].SetValue(.35f);
                Content.Parameters["middleTextureUnblendedMax"].SetValue(.4f);
                Content.Parameters["topTextureUnblendedMin"].SetValue(.45f);

                Content.Parameters["multiTextureEnabled"].SetValue(true);

                Content.Parameters["du"].SetValue(1.0f / SizeX);
                Content.Parameters["dv"].SetValue(1.0f / SizeY);
                Content.Parameters["dx"].SetValue(SizeX / NumIntervalsX);
                Content.Parameters["dy"].SetValue(SizeY / NumIntervalsY);
            }
        }
        
        public override object CreateType(Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService graphics, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            return content.Load<Effect>(EffectPath);
        }
    }
}
