using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace RC.Engine.Rendering
{
    public class RCModel : RCSpatial
    {
        private string _assetPath;
        private Model _model;

        public RCModel(string assetPath)
        {
            _assetPath = assetPath;
        }

        public override void  Load(GraphicsDevice device, ContentManager content)
        {
            _model = content.Load<Model>(_assetPath);            
        }

        public override void Unload()
        {
            _model = null;
        }

        public override void Draw(IRCRenderManager render)
        {
            render.Render(this.Render);
        }

        private void Render(IRCRenderManager render)
        {
            
           Matrix[] transforms = new Matrix[_model.Bones.Count];
           _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                
                render.SetWorld(WorldTrans * transforms[mesh.ParentBone.Index]);

                // Set the index buffer on the device once per mesh
                render.Graphics.Indices = mesh.IndexBuffer;


                // Each mesh is made of parts (grouped by texture, etc.)
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    BasicEffect currentEffect = part.Effect as BasicEffect;
                    
                    if (currentEffect == null)
                    {
                        throw new NotSupportedException("Shader in model is not supported");
                    }

                    render.SetEffectMaterial(
                        currentEffect.AmbientLightColor,
                        currentEffect.DiffuseColor,
                        currentEffect.SpecularColor,
                        currentEffect.SpecularPower,
                        currentEffect.EmissiveColor,
                        currentEffect.Alpha
                        );


                    // Change the device settings for each part to be rendered
                    render.Graphics.VertexDeclaration = part.VertexDeclaration;
                    render.Graphics.Vertices[0].SetSource(
                        mesh.VertexBuffer,
                        part.StreamOffset,
                        part.VertexStride
                    );

                    // Make sure we use the texture for the current part also
                    render.Graphics.Textures[0] = currentEffect.Texture;

                    // Finally draw the actual triangles on the screen
                    render.Graphics.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        part.BaseVertex, 0,
                        part.NumVertices,
                        part.StartIndex,
                        part.PrimitiveCount
                    );

                }
            }
        }

        protected override void UpdateWorldBound()
        {
            // Could update the bonding volume of the model by concatinating all child mesh bounding spheres.
        }
    }
}
