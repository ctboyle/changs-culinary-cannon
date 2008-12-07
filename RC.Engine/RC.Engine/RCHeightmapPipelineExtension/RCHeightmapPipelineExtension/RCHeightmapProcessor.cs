using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;

// TODO: replace these with the processor input and output types.

namespace RCHeightmapPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type Texture2DContent to RC.RCHeightMap. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// </summary>
    [ContentProcessor(DisplayName = "RCHeightmapProcessor")]
    public class RCHeightmapProcessor : ContentProcessor<Texture2DContent, RCHeightMapInfo>
    {
        public override RCHeightMapInfo Process(Texture2DContent input,
                                               ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh("terrain");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));

            

            PixelBitmapContent<float> heightfield;
            heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the terrain mapping.
            float[,] mapping = new float[heightfield.Width, heightfield.Height];
            for (int yCoordinate = 0; yCoordinate < heightfield.Height; yCoordinate++)
            {
                for (int xCoordinate = 0; xCoordinate < heightfield.Width; xCoordinate++)
                {
                    mapping[xCoordinate, yCoordinate] = heightfield.GetPixel(xCoordinate, yCoordinate);
                }
            }

            ExternalReference<Texture2DContent> tex = 
                new ExternalReference<Texture2DContent>(input.Identity.SourceFilename);
            tex = context.BuildAsset<Texture2DContent, Texture2DContent>(tex, "");

            
            return new RCHeightMapInfo(tex, mapping);
        }
    }
}