using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RC.Engine.GraphicsManagement;
using RC.Engine.Cameras;
using RC.Engine.Rendering;

namespace RC.Engine.SceneManagement
{
    public class RCScene : IRCManager
    {
        protected string _cameraLabel;
        protected RCSpatial _sceneRoot;
        protected bool _isLoaded;

        public RCCamera Camera
        {
            get { return RCCameraManager.GetCamera(_cameraLabel); }
        }

        public string SceneCameraLabel
        {
            get { return _cameraLabel; }
            set { _cameraLabel = value; }
        }

        public RCSpatial SceneRoot
        {
            get { return _sceneRoot; }
            set { _sceneRoot = value; }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }

        public RCScene(
            RCSpatial sceneRoot,
            string cameraLabel
            )
        {
            _isLoaded = false;
            _sceneRoot = sceneRoot;
            _cameraLabel = cameraLabel;

        }

        public virtual void Load(IServiceProvider services)
        {
            if (_sceneRoot == null) return;

            _sceneRoot.LoadContent(services);
            _isLoaded = true;
        }

        public virtual void Unload()
        {
            if (_sceneRoot == null) return;
            _sceneRoot.UnloadContent();
        }


        public void Draw(
            GraphicsDevice graphicsDevice
            )
        {
            if (_cameraLabel != null && _sceneRoot != null)
            {
                RCCameraManager.SetActiveCamera(_cameraLabel);
                RCRenderManager.DrawScene(_sceneRoot);
            }
        }

        public virtual void Update(
            GameTime gameTime
            )
        {
            if (_sceneRoot != null)
            {
                _sceneRoot.UpdateGS(gameTime, true);
            }
        }


    }
}
