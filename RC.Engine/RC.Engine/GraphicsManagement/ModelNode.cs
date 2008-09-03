using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using RC.Engine.Cameras;

namespace RC.Engine.GraphicsManagement
{
    public class RCModelNode : RCNode
    {
        public delegate void ApplyEffect(RCCamera camera, RCNode callingNode, ModelMesh mesh, Effect effect);

        protected string _assetName = String.Empty;
        protected Model _model = null;
        protected ApplyEffect _applyEffect = null;

        public RCModelNode(string assetName, ApplyEffect applyEffect)
        {
            _assetName = assetName;
            _applyEffect = applyEffect;
        }

        public string AssetName
        {
            get { return _assetName; }
            set { _assetName = value; }
        }

        public override void LoadContent(ContentManager content, IServiceProvider services)
        {
            _model = content.Load<Model>(_assetName);
            base.LoadContent(content, services);
        }

        public override void Draw(GameTime gameTime, IServiceProvider services)
        {
            Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in _model.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (Effect effect in mesh.Effects)
                {
                    if (_applyEffect == null) break;

                    IRCCameraManager camera = services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;
                    _applyEffect(camera.ActiveCamera, this, mesh, effect);
                }

                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime, services);
        }
    }
}