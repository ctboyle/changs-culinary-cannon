using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using TRead = RC.Content.Heightmap.RCHeightMap;

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


            return new RCHeightMap(textureMap, mapping);
        }
    }

    public class RCHeightMap
    {
        private float[,] mapping;
        private Texture2D textureMapping;

        public RCHeightMap(Texture2D textureMap, float[,] heightMapping)
        {
            Mapping = heightMapping;
            TextureMapping = textureMap;
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
            set
            {
                textureMapping = value;
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
            set
            {
                mapping = value;
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
    }
}