using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using RC.Engine.ContentManagement.ModelReader;
using RC.Engine.GraphicsManagement.Model;
using RC.Engine.GraphicsManagement;
using RC.Engine.Rendering;
using RC.ModelProcessor;
using System.IO;

namespace RC.ModelProcessor
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "RC.ModelProcessor.RCModelProcessor")]
    public class RCModelProcessor : ContentProcessor<NodeContent, RCSceneNode>
    {
        ContentProcessorContext context;
        RCSceneNode model = new RCSceneNode();


        public override RCSceneNode Process(NodeContent input, ContentProcessorContext context)
        {
            this.context = context;
            ProcessNode(input);

            return model;
        }

        private void ProcessNode(NodeContent node)
        {
            MeshContent meshContent = node as MeshContent;
            
            if (meshContent != null)
            {
                RCSceneNode mesh = new RCSceneNode();
                mesh.LocalTrans = node.Transform;

                foreach (GeometryContent geometryContent in meshContent.Geometry)
                {
                    RCGeometryContent childPart = ProcessGeometry(geometryContent);
                    mesh.AddChild(childPart);
                }

                model.AddChild(mesh);
            }

            // Recurse over any child nodes.
            foreach (NodeContent child in node.Children)
            {
                ProcessNode(child);
            }

        }

        private RCGeometryContent ProcessGeometry(GeometryContent geometryContent)
        {
            int triangleCount = geometryContent.Indices.Count / 3;
            int vertexCount = geometryContent.Vertices.VertexCount;

            // Flatten the flexible input vertex channel data into
            // a simple GPU style vertex buffer byte array.
            VertexBufferContent vertexBufferContent;
            VertexElement[] vertexElements;


            geometryContent.Vertices.CreateVertexBuffer(out vertexBufferContent,
                                                 out vertexElements,
                                                 context.TargetPlatform);

            ProcessMaterial(geometryContent.Material);


            return new RCGeometryContent(
                vertexElements,
                vertexBufferContent,
                geometryContent.Indices,
                vertexCount,
                triangleCount);

        }

        private void ProcessMaterial(MaterialContent materialContent)
        {
            EffectMaterialContent effectContent = new EffectMaterialContent();


            string effectPath = Path.GetFullPath("Content\\ShaderEffects\\Texture.fx");
            effectContent.Effect = new ExternalReference<EffectContent>(effectPath);

           

        }

        
        
    }










    [ContentTypeWriter]
    public class RCModelWriter : ContentTypeWriter<RCModelContent>
    {
        
        protected override void  Write(ContentWriter output, RCModelContent value)
        {
            output.WriteRawObject<ModelContent>(value.Content);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(RCModelContent).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(RCModelReader).AssemblyQualifiedName;

            // Old Way (See edit notes at bottom):
            // "BBoxImporter.BBoxReader, BBoxImporter, Version=1.0.0.0, Culture=neutral";
        }
    }

}