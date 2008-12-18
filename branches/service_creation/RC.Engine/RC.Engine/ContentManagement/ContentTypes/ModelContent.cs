using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.ContentManagement;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;
using RC.Engine.GraphicsManagement.BoundingVolumes;
using Microsoft.Xna.Framework.Content;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.ContentManagement.ContentTypes
{
    /// <summary>
    /// A model content type.
    /// </summary>
    public class RCModelContent : RCContent<RCSceneNode>
    {
        private string _assetName;

        /// <summary>
        /// Creates a new instance of the model content by asset name.
        /// </summary>
        /// <param name="assetName">The asset name.</param>
        public RCModelContent(string assetName)
            : base()
        {
            _assetName = assetName;
        }

        /// <summary>
        /// Creates a new instance of the content.
        /// </summary>
        /// <param name="graphics">The graphics device service.</param>
        /// <param name="content">The content manager.</param>
        /// <returns>The instance of content.</returns>
        protected override object OnCreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            Model model = content.Load<Model>(_assetName);

            RCSceneNode modelRoot = ConvertToScene(model);
            return modelRoot;
        }

        private RCSceneNode ConvertToScene(Model xnaModel)
        {
            RCSceneNode model = new RCSceneNode();

            foreach (ModelMesh xnaMesh in xnaModel.Meshes)
            {
                foreach (ModelMeshPart xnaPart in xnaMesh.MeshParts)
                {
                    RCVertexRefrence vertexRefrence = new RCVertexRefrence(
                        xnaMesh.IndexBuffer,
                        xnaMesh.VertexBuffer,
                        xnaPart.VertexDeclaration,
                        xnaPart.VertexStride,
                        xnaPart.StreamOffset,
                        xnaPart.StartIndex,
                        xnaPart.BaseVertex,
                        xnaPart.NumVertices,
                        xnaPart.PrimitiveCount);

                    // Create the scene graph object that holds the drawable data.
                    RCGeometry newPart = new RCGeometry();
                    newPart.PartData = vertexRefrence;

                    newPart.Effects.Add(new RCModelPartEffect(xnaPart.Effect));

                    newPart.LocalBound = new RCBoundingSphere(xnaMesh.BoundingSphere);
                    newPart.LocalTrans = xnaMesh.ParentBone.Transform;
                    model.AddChild(newPart);
                }
            }

            return model;
        }
    }
}
