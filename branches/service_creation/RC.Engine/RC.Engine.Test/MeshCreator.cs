using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;
using RC.Engine.ContentManagement;
using RC.Content.Heightmap;
using Microsoft.Xna.Framework;

namespace RC.Engine.Test
{
    class MeshCreator
    {
        public static RCGeometry CreateObject(IGraphicsDeviceService graphics)
        {
            int[] indicies = new int[] { 0, 2, 3, 0, 1, 2 };

            float[] vertices = new float[]
            {   
                -1, -1, 0,
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

            RCVertexBuffer vBuffer = new RCVertexBuffer(vAttrib, 4);
            vBuffer.SetData(ElementType.Position, vertices);
            vBuffer.SetData(ElementType.Texture, texCoords);
            vBuffer.SetData(ElementType.Normal, normals);

            RCIndexBuffer iBuffer = new RCIndexBuffer( 6);
            iBuffer.SetData(indicies);

            iBuffer.Enable(graphics);
            vBuffer.Enable(graphics);
            RCVertexRefrence vertexRefrence = new RCVertexRefrence(
                iBuffer.IndexBuffer,
                vBuffer.VertexBuffer,
                vBuffer.VertexDeclaration,
                vBuffer.VertexSize,
                0, 0, 0,
                vBuffer.NumVertices,
                iBuffer.NumIndicies / 3);


            RCGeometry geometry = new RCGeometry();
            geometry.PartData = vertexRefrence;

            RCMaterialState material = new RCMaterialState();
            material.Ambient = new Color(255, 0, 0, 255);
            material.Diffuse = new Color(255, 0, 0, 255);
            material.Specular = new Color(255, 255, 255, 255);
            material.Shininess = 15.0f;
            material.Alpha = 0.0f;

            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>("Content\\Textures\\smiley");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>("Content\\Textures\\seattle");
            RCTextureEffect effect1 = new RCTextureEffect(texture1);
            RCTextureEffect effect2 = new RCTextureEffect(texture2);
           
            geometry.AddEffect(effect1);
            geometry.AddEffect(effect2);

            geometry.GlobalStates.Add(material);

            return geometry;
        }
    }
}
