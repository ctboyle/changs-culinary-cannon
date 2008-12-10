using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using RC.Engine.GraphicsManagement;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Ninject.Core;
using Microsoft.Xna.Framework;

namespace RC.Engine.ContentManagement.ContentPipeline.Readers
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    
    public abstract class RCSceneReader<T> : ContentTypeReader<T> where T : ISpatial
    {
        private IGraphicsDeviceService _graphics = null;

        [Inject]
        public IGraphicsDeviceService Graphics
        {
            set { _graphics = value; }
            get { return _graphics; }
        }

        protected override T Read(ContentReader input, T existingInstance)
        {
            T spatial = existingInstance;
            spatial.LocalTrans = input.ReadObject<Matrix>();
            return spatial;
        }
    }


    public class RCSpatialReader : RCSceneReader<RCSpatial>
    {
        protected override RCSpatial Read(ContentReader input, RCSpatial existingInstance)
        {
            return base.Read(input, existingInstance);
        }
    }

    public class RCSceneNodeReader : RCSceneReader<RCSceneNode>
    {
        protected override RCSceneNode Read(ContentReader input, RCSceneNode existingInstance)
        {
            
            RCSceneNode node = existingInstance ?? new RCSceneNode();
            base.Read(input, node);
            

            int countChildren = input.ReadInt32();

            for (int iChild = 0; iChild < countChildren; iChild++ )
            {
                RCSpatial child;
                child = input.ReadObject<RCSpatial>();
                node.AddChild(child);
            }
            return node;
        }
    }


    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    public class RCGeometryReader : RCSceneReader<RCGeometry>
    {
        protected override RCGeometry Read(ContentReader input, RCGeometry existingInstance)
        {
            RCGeometry part = new RCGeometry();
            base.Read(input, part);

            int vertexCount = input.ReadInt32();
            int triangleCount = input.ReadInt32();
            int stride = input.ReadInt32();

            VertexDeclaration vertexDeclaration = input.ReadObject<VertexDeclaration>();
            VertexBuffer vBuffer = input.ReadObject<VertexBuffer>();
            IndexBuffer iBuffer = input.ReadObject<IndexBuffer>();

            RCVertexRefrence refrence = new RCVertexRefrence(
                iBuffer,
                vBuffer,
                vertexDeclaration,
                stride,
                0,0,0,                
                vertexCount,
                triangleCount);

            part.PartData = refrence;

            return part;
        }
    }

}
