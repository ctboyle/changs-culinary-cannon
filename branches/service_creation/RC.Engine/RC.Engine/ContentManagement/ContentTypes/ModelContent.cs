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
    public class RCModelContent : RCContent<RCSceneNode>
    {
        private string _assetName;

        public RCModelContent(IRCContentRequester contentRqst, string assetName)
            : base(contentRqst)
        {
            _assetName = assetName;
        }

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
                RCSceneNode meshNode = new RCSceneNode();
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

                    newPart.Effects.Add(new RCModelPartEffect(xnaPart.Effect, _contentMgr ));

                    newPart.LocalBound = new RCBoundingSphere(xnaMesh.BoundingSphere);

                    meshNode.AddChild(newPart);
                }

                meshNode.LocalTrans = xnaMesh.ParentBone.Transform;
                model.AddChild(meshNode);
            }

            return model;
        }

    }
}
