using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using TWrite = RC.Content.Pipeline.HeightMap.RCHeightMapInfo;
using TReader = RC.Content.Heightmap.RCHeightMapReader;
using TRuntime = RC.Content.Heightmap.RCHeightMap;

namespace RC.Content.Pipeline.HeightMap
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class RCHeightMapWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.Mapping.GetLength(0));
            output.Write(value.Mapping.GetLength(1));

            for (int xCoordinate = 0; xCoordinate < value.Mapping.GetLength(0); xCoordinate++)
            {
                for (int yCoordinate = 0; yCoordinate < value.Mapping.GetLength(1); yCoordinate++)
                {
                    output.Write(value.Mapping[xCoordinate, yCoordinate]);
                }
            }

            output.WriteExternalReference<Texture2DContent>(value.TextureMappingContent);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TRuntime).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TReader).AssemblyQualifiedName;
        }
    }
}