using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace RC
{
    public class RCHeightMap
    {
        private float[,] mapping;
        private Texture2D textureMapping;

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


        public RCHeightMap(Texture2D textureMap, float[,] heightMapping)
        {
            Mapping = heightMapping;
            TextureMapping = textureMap;
        }
    }
}
