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
        public static RCGeometry CreateObject(IGraphicsDeviceService graphics, IRCContentRequester contentRqst)
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

            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\smiley");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\seattle");
            RCTextureEffect effect1 = new RCTextureEffect(contentRqst, texture1);
            RCTextureEffect effect2 = new RCTextureEffect(contentRqst, texture2);
           
            geometry.AddEffect(effect1);
            geometry.AddEffect(effect2);

            geometry.GlobalStates.Add(material);

            return geometry;
        }

        public static RCGeometry CreateHeightMapObject(IGraphicsDeviceService graphics, IRCContentRequester contentRqst, RCContent<RCHeightMap> heightMap)
        {
            int numVertices = (HeightMapEffect.NumIntervalsX + 1) * (HeightMapEffect.NumIntervalsY + 1);
            int numIndices = 6 * HeightMapEffect.NumIntervalsX * HeightMapEffect.NumIntervalsY;

            int[] indices = new int[numIndices];
            float[] vertices = new float[3 * numVertices];
            float[] texCoords = new float[2 * numVertices];
            float[] normals = new float[3 * numVertices];

            float dx = HeightMapEffect.SizeX / HeightMapEffect.NumIntervalsX;
            float dy = HeightMapEffect.SizeY / HeightMapEffect.NumIntervalsY;
            float txinc = 1.0f / HeightMapEffect.NumIntervalsX;
            float tyinc = 1.0f / HeightMapEffect.NumIntervalsY;

            Vector3 position, normal = Vector3.Zero;
            Vector2 texture = Vector2.Zero;

            int vertexIdx = 0, fvertexIdx = 0, a = 0, b = 0, c = 0;

            texture.X = 0.0f;
            normal.Z = 1.0f;
            position.Z = 0;

            for (int i = 0; i <= HeightMapEffect.NumIntervalsX; i++)
            {
                position.X = (-HeightMapEffect.SizeX / 2.0f) + i * dx;
                texture.Y = 1.0f;

                for (int j = 0; j <= HeightMapEffect.NumIntervalsY; j++)
                {
                    position.Y = (-HeightMapEffect.SizeY / 2.0f) + j * dy;



                    vertices[(3*vertexIdx)+0] = position.X;
                    vertices[(3*vertexIdx)+1] = position.Y;
                    vertices[(3*vertexIdx)+2] = position.Z;

                    normals[(3 * vertexIdx) + 0] = normal.X;
                    normals[(3 * vertexIdx) + 1] = normal.Y;
                    normals[(3 * vertexIdx) + 2] = normal.Z;

                    texCoords[(2 * vertexIdx) + 0] = texture.X;
                    texCoords[(2 * vertexIdx) + 1] = texture.Y;

                    if (i < HeightMapEffect.NumIntervalsX && j < HeightMapEffect.NumIntervalsY)
                    {
                        indices[fvertexIdx++] = (vertexIdx);
                        indices[fvertexIdx++] = (vertexIdx + HeightMapEffect.NumIntervalsY + 1);
                        indices[fvertexIdx++] = (vertexIdx + HeightMapEffect.NumIntervalsY + 2);
                        indices[fvertexIdx++] = (vertexIdx);
                        indices[fvertexIdx++] = (vertexIdx + HeightMapEffect.NumIntervalsY + 2);
                        indices[fvertexIdx++] = (vertexIdx + 1);
                    }

                    vertexIdx++;
                    texture.Y -= tyinc;
                }
                texture.X += txinc;
            }

            RCVertexAttributes vAttrib = new RCVertexAttributes();
            vAttrib.SetElementChannels(ElementType.Position, RCVertexAttributes.ChannelCount.Three);
            vAttrib.SetElementChannels(ElementType.Texture, RCVertexAttributes.ChannelCount.Two);
            vAttrib.SetElementChannels(ElementType.Normal, RCVertexAttributes.ChannelCount.Three);

            RCVertexBuffer vBuffer = new RCVertexBuffer(vAttrib, numVertices);
            vBuffer.SetData(ElementType.Position, vertices);
            vBuffer.SetData(ElementType.Texture, texCoords);
            vBuffer.SetData(ElementType.Normal, normals);

            RCIndexBuffer iBuffer = new RCIndexBuffer(numIndices);
            iBuffer.SetData(indices);

            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\grass");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\rock");
            RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(contentRqst, "Content\\Textures\\snow");
            HeightMapEffect effect = new HeightMapEffect(contentRqst, heightMap, texture1, texture2, texture3);

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
            geometry.AddEffect(effect);

            return geometry;
        }
    }
}
