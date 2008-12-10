using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using TRead = RC.Content.Heightmap.RCHeightMap;
using RC.Engine.GraphicsManagement;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;

namespace RC.Content.Heightmap
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to read the specified data type from binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    public class RCHeightMapReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            int xSize = 0;
            int ySize = 0;

            //read the mapping's dimensions here
            xSize = input.ReadInt32();
            ySize = input.ReadInt32();

            float[,] mapping = new float[xSize, ySize];

            for (int xCoordinate = 0; xCoordinate < xSize; xCoordinate++)
            {
                for (int yCoordinate = 0; yCoordinate < ySize; yCoordinate++)
                {
                    mapping[xCoordinate, yCoordinate] = input.ReadSingle();
                }
            }
            
            //read the mapping here

            Texture2D textureMap;
            //textureMap = input.ReadObject<Texture2D>();
            textureMap = input.ReadExternalReference<Texture2D>();
            //read the model here
            
            return new RCHeightMap(textureMap, mapping, 1);
        }
    }

    public class RCHeightMap : RCSceneNode
    {
        public const int NumIntervalsX = 100;
        public const int NumIntervalsZ = 100;
        public const float SizeX = 2;
        public const float SizeZ = 2;

        private float[,] mapping = null;
        private RCGeometry geometry = null;
        private Texture2D textureMapping = null;
        private int[] indices = null;
        private float[] vertices = null;
        private float[] texCoords = null;
        private float[] normals = null;
        private int numVertices = 0;
        private int numIndices = 0;
        private float scaling = 1;

        public float Scaling
        {
            get { return scaling; }
            set { scaling = value; }
        }

        public RCHeightMap(Texture2D textureMap, float[,] heightMapping, float scaling)
        {
            mapping = heightMapping;
            textureMapping = textureMap;
        }

        /// <summary>
        /// Provides accessors for textureMapping.
        /// </summary>
        public Texture2D TextureMapping
        {
            get
            {
                return textureMapping;
            }
        }

        /// <summary>
        /// Provides accessors for mapping.
        /// </summary>
        public float[,] Mapping
        {
            get
            {
                return mapping;
            }
        }

        #region Implicit conversions
        /// <summary>
        /// Implicitly converts <paramref name="heightMap"/> to a 
        /// 2-dimensional array of floats.
        /// </summary>
        /// <param name="heightMap">The RCHeightMap to convert</param>
        /// <returns>The 2-dimensional array of floats contained
        ///          within <paramref name="heightMap"/>.</returns>
        public static implicit operator float[,](RCHeightMap heightMap)
        {
            return heightMap.Mapping;
        }

        /// <summary>
        /// Implicitly converts <paramref name="heightMap"/> to a 
        /// Texture2D object.
        /// </summary>
        /// <param name="heightMap">The RCHeightMap to convert</param>
        /// <returns>The Texture2D contained 
        ///          within <paramref name="heightMap"/>.</returns>
        public static implicit operator Texture2D(RCHeightMap heightMap)
        {
            return heightMap.TextureMapping;
        }
        #endregion Implicit conversions
        
        public override void Draw(IRCRenderManager render, RC.Engine.ContentManagement.IRCContentRequester contentRqst)
        {
            if (geometry == null)
            {
                SetupData();
            }

            RCVertexRefrence vertexReference = SetupVertexReference(render.Graphics);

            geometry = new RCGeometry();
            geometry.PartData = vertexReference;

            foreach (RCEffect effect in Effects)
            {
                geometry.AddEffect(effect);
            }

            render.Draw(geometry);
            base.Draw(render, contentRqst);
        }
        
        private void SetupData()
        {
            float dx = SizeX / NumIntervalsX;
            float dz = SizeZ / NumIntervalsZ;
            float txinc = 1.0f / NumIntervalsX;
            float tyinc = 1.0f / NumIntervalsZ;
            //scaling = 1;

            Vector3 position, normal = Vector3.Zero;
            Vector2 texture = Vector2.Zero;

            int vertexIdx = 0, fvertexIdx = 0;

            numVertices = (NumIntervalsX + 1) * (NumIntervalsZ + 1);
            numIndices = 6 * NumIntervalsX * NumIntervalsZ;

            indices = new int[numIndices];
            vertices = new float[3 * numVertices];
            texCoords = new float[2 * numVertices];
            normals = new float[3 * numVertices];

            texture.X = 0.0f;
            normal.Y = 1.0f;
            position.Y = 0;

            for (int i = 0; i <= NumIntervalsX; i++)
            {
                position.X = (-SizeX / 2.0f) + i * dx;
                texture.Y = 1.0f;

                for (int j = 0; j <= NumIntervalsZ; j++)
                {
                    position.Z = (-SizeZ / 2.0f) + j * dz;

                    vertices[(3 * vertexIdx) + 0] = scaling * position.X;
                    vertices[(3 * vertexIdx) + 1] = scaling * 
                        Mapping[(int)(127 * texture.X), (int)(127 * texture.Y)];
                    vertices[(3 * vertexIdx) + 2] = scaling * position.Z;
                        

                    normals[(3 * vertexIdx) + 0] = normal.X;
                    normals[(3 * vertexIdx) + 1] = normal.Y;
                    normals[(3 * vertexIdx) + 2] = normal.Z;

                    texCoords[(2 * vertexIdx) + 0] = texture.X*2 * scaling;
                    texCoords[(2 * vertexIdx) + 1] = texture.Y*2 * scaling;

                    if (i < NumIntervalsX && j < NumIntervalsZ)
                    {
                        indices[fvertexIdx++] = (vertexIdx);
                        indices[fvertexIdx++] = (vertexIdx + NumIntervalsZ + 1);
                        indices[fvertexIdx++] = (vertexIdx + NumIntervalsZ + 2);
                        indices[fvertexIdx++] = (vertexIdx);
                        indices[fvertexIdx++] = (vertexIdx + NumIntervalsZ + 2);
                        indices[fvertexIdx++] = (vertexIdx + 1);
                    }

                    vertexIdx++;
                    texture.Y -= tyinc;
                }
                texture.X += txinc;
            }

            
        }

        private RCVertexRefrence SetupVertexReference(IGraphicsDeviceService graphics)
        {
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

            iBuffer.Enable(graphics);
            vBuffer.Enable(graphics);

            RCVertexRefrence vertexRefrence = new RCVertexRefrence(
                iBuffer.IndexBuffer,
                vBuffer.VertexBuffer,
                vBuffer.VertexDeclaration,
                vBuffer.VertexSize,
                0, 0, 0,
                vBuffer.NumVertices,
                iBuffer.NumIndicies / 3
                );

            return vertexRefrence;
        }
    }
}