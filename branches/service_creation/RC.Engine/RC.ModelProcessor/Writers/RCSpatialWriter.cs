using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using RC.Engine.GraphicsManagement;
using RC.Engine.ContentManagement.ContentPipeline.Readers;

namespace RC.ModelProcessor.Writers
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    
    public abstract class RCSceneWriter<T> : ContentTypeWriter<T> where T: ISpatial
    {
        protected override void  Write(ContentWriter output, T value)
        {
            output.WriteObject(value.LocalTrans);
        }
    }

    [ContentTypeWriter]
    public class RCSpatialWriter : RCSceneWriter<RCSpatial>
    {
        protected override void Write(ContentWriter output, RCSpatial value)
        {
            base.Write(output, value);
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(RCSpatial).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(RCSpatialReader).AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class RCSceneNodeWriter : RCSceneWriter<RCSceneNode>
    {
        protected override void Write(ContentWriter output, RCSceneNode value)
        {
            base.Write(output, value);

            List<ISpatial> children = value.GetChildren();
            output.Write(children.Count);

            foreach (ISpatial child in children)
            {
                output.WriteObject(child);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(RCSceneNode).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(RCSceneNodeReader).AssemblyQualifiedName;
        }
    }


    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class RCGeometryWriter : RCSceneWriter<RCGeometryContent>
    {
        protected override void Write(ContentWriter output, RCGeometryContent value)
        {
            base.Write(output, value);

            output.Write(value.VertexCount);
            output.Write(value.TriangleCount);
            output.Write(VertexDeclaration.GetVertexStrideSize(value.VertexElements, 0));


            output.WriteObject(value.VertexElements);
            output.WriteObject(value.VertexBuffer);
            output.WriteObject(value.IndexBuffer);

        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(RCGeometry).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(RCGeometryReader).AssemblyQualifiedName;
        }
    }


}
