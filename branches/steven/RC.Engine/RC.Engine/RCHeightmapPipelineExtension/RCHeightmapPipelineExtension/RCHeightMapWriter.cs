using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using RC;


namespace RCHeightmapPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class RCHeightMapWriter : ContentTypeWriter<RCHeightMapInfo>
    {
        protected override void Write(ContentWriter output, RCHeightMapInfo value)
        {
            output.Write(value.Mapping.GetLength(0));
            output.Write(value.Mapping.GetLength(1));

            for(int xCoordinate=0;xCoordinate<value.Mapping.GetLength(0);xCoordinate++)
            {
                for(int yCoordinate=0;yCoordinate<value.Mapping.GetLength(1);yCoordinate++)
                {
                    output.Write(value.Mapping[xCoordinate, yCoordinate]);
                }
            }

            
            output.WriteExternalReference<Texture2DContent>(value.TextureMappingContent);
            
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(RC.RCHeightMap).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return typeof(RC.RCHeightMapReader).AssemblyQualifiedName;
            return "RC.RCHeightMapReader, RCHeightMap";
        }
    }
}
