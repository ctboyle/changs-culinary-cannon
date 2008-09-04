using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;

namespace RC.Engine.Test
{
    class Wedge : RC.Engine.GraphicsManagement.RCSpatial
    {
        private Model _wedgeModel = null;
        private ModelMesh _currentMesh = null;

        public override void Load(IServiceProvider services)
        {
            ContentManager content = services.GetService(typeof(ContentManager)) as ContentManager;
            _wedgeModel = content.Load<Model>(@"Content/enemy");
        }

        public override void Unload()
        {
            _wedgeModel = null;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, RC.Engine.Rendering.IRCRenderManager render)
        {
            render.SetWorld(this.WorldTrans);

            foreach (ModelMesh mesh in _wedgeModel.Meshes)
            {
                _currentMesh = mesh;

                render.Render(graphicsDevice, RenderModel);
            }
        }

        protected override void UpdateWorldBound()
        {
           
        }

        protected void RenderModel(RC.Engine.Rendering.IRCRenderManager render, GraphicsDevice graphicsDevice)
        {
            _currentMesh.Draw();
            //// Each mesh is made of parts (grouped by texture, etc.)
            //foreach (ModelMeshPart part in _currentMesh.MeshParts)
            //{
            //    BasicEffect currentEffect = (BasicEffect)part.Effect;

            //    render.SetEffectMaterial(
            //        currentEffect.AmbientLightColor,
            //        currentEffect.DiffuseColor,
            //        currentEffect.SpecularColor,
            //        currentEffect.SpecularPower,
            //        currentEffect.EmissiveColor,
            //        currentEffect.Alpha
            //        );


            //    // Change the device settings for each part to be rendered
            //    graphicsDevice.VertexDeclaration = part.VertexDeclaration;
            //    graphicsDevice.Vertices[0].SetSource(
            //        _currentMesh.VertexBuffer,
            //        part.StreamOffset,
            //        part.VertexStride
            //    );

            //    // Make sure we use the texture for the current part also
            //    graphicsDevice.Textures[0] = currentEffect.Texture;

            //    // Finally draw the actual triangles on the screen
            //    graphicsDevice.DrawIndexedPrimitives(
            //        PrimitiveType.TriangleList,
            //        part.BaseVertex, 0,
            //        part.NumVertices,
            //        part.StartIndex,
            //        part.PrimitiveCount
            //    );
            // }
        }
    }
}