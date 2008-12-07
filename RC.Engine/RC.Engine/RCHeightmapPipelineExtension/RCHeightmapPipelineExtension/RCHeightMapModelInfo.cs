//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Processors;
//using Microsoft.Xna.Framework.Graphics;
//using HeightmapProcessor;

//namespace RCHeightmapPipelineExtension
//{
//    public class RCHeightMapModelInfo
//    {
//        private float[,] mapping;
//        private ExternalReference<Texture2DContent> textureMappingContent;
//        private ExternalReference<HeightMapInfoContent> mapInfoContent;

        

//        /// <summary>
//        /// Provides accessors for textureMapping.
//        /// </summary>
//        public ExternalReference<Texture2DContent> TextureMappingContent
//        {
//            get
//            {
//                return textureMappingContent;
//            }
//            set
//            {
//                textureMappingContent = value;
//            }
//        }

//        /// <summary>
//        /// Provides accessors for mapping.
//        /// </summary>
//        public float[,] Mapping
//        {
//            get
//            {
//                return mapping;
//            }
//            set
//            {
//                mapping = value;
//            }
//        }

//        public ExternalReference<HeightMapInfoContent> MapInfoContent
//        {
//            get { return mapInfoContent; }
//            set { mapInfoContent = value; }
//        }

//        //#region Implicit conversions
//        ///// <summary>
//        ///// Implicitly converts <paramref name="heightMap"/> to a 
//        ///// 2-dimensional array of floats.
//        ///// </summary>
//        ///// <param name="heightMap">The RCHeightMap to convert</param>
//        ///// <returns>The 2-dimensional array of floats contained
//        /////          within <paramref name="heightMap"/>.</returns>
//        //public static implicit operator float[,](RCHeightMap heightMap)
//        //{
//        //    return heightMap.Mapping;
//        //}

//        ///// <summary>
//        ///// Implicitly converts <paramref name="heightMap"/> to a 
//        ///// ModelContent object.
//        ///// </summary>
//        ///// <param name="heightMap">The RCHeightMap to convert</param>
//        ///// <returns>The ModelContent contained 
//        /////          within <paramref name="heightMap"/>.</returns>
//        //public static implicit operator ModelContent(RCHeightMap heightMap)
//        //{
//        //    return heightMap.ModelContent;
//        //}
//        //#endregion Implicit conversions


//        public RCHeightMapModelInfo(ExternalReference<Texture2DContent> textureMapContent, 
//                                    float[,] heightMapping, ExternalReference<HeightMapInfoContent> mapInfContent)
//        {
//            Mapping = heightMapping;
//            TextureMappingContent = textureMapContent;
//            mapInfoContent = mapInfContent;
//        }
//    }
//}
