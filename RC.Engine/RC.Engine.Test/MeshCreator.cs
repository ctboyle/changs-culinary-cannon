using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;
using Ninject.Core;
using RC.Engine.ContentManagement;

namespace RC.Engine.Test
{
    class MeshCreator
    {
        public static RCGeometry CreateObject(IGraphicsDeviceService graphics, IRCContentRequester content)
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

            RCGeometry geometry = new RCGeometry(graphics, content, iBuffer, vBuffer);

            RCMaterialState material = new RCMaterialState();
            material.Ambient = new Color(255, 0, 0, 255);
            material.Diffuse = new Color(255, 0, 0, 255);
            material.Specular = new Color(255, 255, 255, 255);
            material.Shininess = 15.0f;
            material.Alpha = 0.0f;

            RCContent<Texture2D> texture1 = content.RequestContent<Texture2D>("Content\\Textures\\seattle");
            RCContent<Texture2D> texture2 = content.RequestContent<Texture2D>("Content\\Textures\\smiley");
            
            RCTextureEffect effect1 = RCTextureEffect.Create(content, texture1);
            RCTextureEffect effect2 = RCTextureEffect.Create(content, texture2);

            geometry.AddEffect(effect1);
            geometry.AddEffect(effect2);

            geometry.GlobalStates.Add(material);

            return geometry;
        }
    }
}