using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;
using RC.Engine.ContentManagement;
using RC.Content.Heightmap;

namespace RC.Engine.Test
{
    class MeshCreator
    {
        public static RCGeometry CreateObject(IGraphicsDeviceService graphics, IRCContentRequester contentRqst)
        {
            RCGeometry geometry = CreateBasicObject(graphics, contentRqst);

            RCMaterialState material = new RCMaterialState();
            material.Ambient = new Color(255, 0, 0, 255);
            material.Diffuse = new Color(255, 0, 0, 255);
            material.Specular = new Color(255, 255, 255, 255);
            material.Shininess = 15.0f;
            material.Alpha = 0.0f;

            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\smiley");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\seattle");
            RCTextureEffect effect1 = new RCTextureEffect(contentRqst, texture1);
            RCTextureEffect effect2 = new RCTextureEffect(contentRqst, texture2);
           
            geometry.AddEffect(effect1);
            geometry.AddEffect(effect2);

            geometry.GlobalStates.Add(material);

            return geometry;
        }

        public static RCGeometry CreateHeightMapObject(IGraphicsDeviceService graphics, IRCContentRequester contentRqst, RCHeightMap heightMap)
        {
            RCGeometry geometry = CreateBasicObject(graphics, contentRqst);

            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\grass");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\rock");
            RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\snow");
            HeightMapEffect effect = new HeightMapEffect(
                contentRqst, 
                heightMap, 
                texture1, 
                texture2, 
                texture3
                );

            geometry.AddEffect(effect);

            return geometry;
        }

        public static RCGeometry CreateBasicObject(IGraphicsDeviceService graphics, IRCContentRequester contentRqst)
        {
            short[] indicies = new short[] { 0, 2, 3, 0, 1, 2 };

            float[] vertices = new float[]
            {   -1, -1, 0,
                 1, -1, 0,
                 1,  1, 0,
                -1,  1, 0
            };

            float[] texCoords = new float[]
            {   
                0, 1,
                1, 1, 
                1, 0,
                0, 0
                
            };

            float[] normals = new float[]
            {
                0, 0, 1,
                0, 0, 1,
                0, 0, 1,
                0, 0, 1
            };

            RCVertexAttributes vAttrib = new RCVertexAttributes();
            vAttrib.SetElementChannels(ElementType.Position, RCVertexAttributes.ChannelCount.Three);
            vAttrib.SetElementChannels(ElementType.Texture, RCVertexAttributes.ChannelCount.Two);
            vAttrib.SetElementChannels(ElementType.Normal, RCVertexAttributes.ChannelCount.Three);

            RCVertexBuffer vBuffer = new RCVertexBuffer(graphics, vAttrib, 4);
            vBuffer.SetData(ElementType.Position, vertices);
            vBuffer.SetData(ElementType.Texture, texCoords);
            vBuffer.SetData(ElementType.Normal, normals);

            RCIndexBuffer iBuffer = new RCIndexBuffer(graphics, 6);
            iBuffer.SetData(indicies);

            RCGeometry geometry = new RCGeometry(iBuffer, vBuffer);

            return geometry;
        }
    }
}