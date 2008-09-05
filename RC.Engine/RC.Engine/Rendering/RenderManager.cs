using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Cameras;
using RC.Engine.GraphicsManagement;
using RC.Engine.Utility;

namespace RC.Engine.Rendering
{
    public delegate void RenderFunc(GraphicsDevice device, IRCRenderManager render);

    public enum DirectionalLightIndex
    {
        Light0 = 0,
        Light1,
        Light2,
        Count
    }

    public interface IRCRenderManager : IRCLoadable
    {
        GraphicsDevice Graphics { get; }
        void EnableDirectionalLight(RCDirectionalLight lightNode);
        void DisableDirectionalLight(RCDirectionalLight lightNode);
        void SetEffectMaterial(
            Vector3 ambient,
            Vector3 diffuse,
            Vector3 specular,
            float specularPower,
            Vector3 emissive,
            float alpha
            );
        void SetTexture(Texture2D texture);
        void TextureMappingEnabled(bool enabled);
        void SetWorld(Matrix world);
        void Render(RenderFunc renderLogic);
        void RenderModel(Model model, RenderFunc renderLogic);
        void DrawScene(RCSpatial sceneRoot);
        void ClearScreen();
    }

    /// <summary>
    /// Central functionality for Rendering the Scene.
    /// </summary>
    internal class RCRenderManager : IRCRenderManager
    {
        private BasicEffect _sceneEffect = null;
        private int _countEnabledLights = 0;
        private Color _clearColor = Color.CornflowerBlue;
        private IRCCameraManager _cameraMgr = null;
        private Game _game = null;

        public RCRenderManager(Game game)
        {
            _game = game;
            _game.Services.AddService(typeof(IRCRenderManager), this);
        }

        public GraphicsDevice Graphics
        {
            get { return _game.GraphicsDevice; }
        }

        public void Load()
        {
            _cameraMgr = _game.Services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;

            IGraphicsDeviceService graphics = _game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;

            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            graphics.GraphicsDevice.RenderState.StencilEnable = true;

            _sceneEffect = new BasicEffect(graphics.GraphicsDevice, null);
        }

        public void Unload()
        {
            _sceneEffect = null;
        }

        /// <summary>
        /// Enables the RenderManagers effect with the lightNode's Index light
        /// and assgins the appropriate effect properties.
        /// </summary>
        public void EnableDirectionalLight(RCDirectionalLight lightNode)
        {
            if (_sceneEffect == null) return;

            BasicDirectionalLight effectLight = null;

            switch (lightNode.LightIndex)
            {
                case DirectionalLightIndex.Light0:
                    effectLight = _sceneEffect.DirectionalLight0;
                    break;
                case DirectionalLightIndex.Light1:
                    effectLight = _sceneEffect.DirectionalLight1;
                    break;
                case DirectionalLightIndex.Light2:
                    effectLight = _sceneEffect.DirectionalLight2;
                    break;
                default:
                    throw (new Exception("Light Index is invalid."));

            }

            if (effectLight.Enabled)
            {
                throw (new Exception("Light is already enabled."));
            }

            // Enable Lighting
            _sceneEffect.LightingEnabled = true;
            effectLight.Enabled = true;


            // Set Effect Light properties to reflect the LightNode
            effectLight.DiffuseColor = lightNode.Diffuse;
            effectLight.SpecularColor = lightNode.Specular;
            effectLight.Direction = lightNode.Direction;

            // increment local count of enabled lights
            _countEnabledLights++;

            if (_countEnabledLights > (int)DirectionalLightIndex.Count)
            {
                throw (new Exception("Tried to enable more lights than RenderManager provides."));
            }
        }


        /// <summary>
        /// Disables the light refrenced by lightNode's index. Disables lighting if
        /// enabled light count goes to zero.
        /// </summary>
        public void DisableDirectionalLight(RCDirectionalLight lightNode)
        {
            if (_sceneEffect == null) return;

            BasicDirectionalLight effectLight = null;

            switch (lightNode.LightIndex)
            {
                case DirectionalLightIndex.Light0:
                    effectLight = _sceneEffect.DirectionalLight0;
                    break;
                case DirectionalLightIndex.Light1:
                    effectLight = _sceneEffect.DirectionalLight1;
                    break;
                case DirectionalLightIndex.Light2:
                    effectLight = _sceneEffect.DirectionalLight2;
                    break;
                default:
                    throw (new Exception("Light Index is invalid."));

            }

            effectLight.Enabled = false;

            _countEnabledLights--;

            // Trun off lighting if there are no lights enabled.
            if (_countEnabledLights <= 0)
            {
                _sceneEffect.LightingEnabled = false;
            }
        }

        /// <summary>
        /// Sets the current material properties to be rendered.
        /// </summary>
        public void SetEffectMaterial(
            Vector3 ambient,
            Vector3 diffuse,
            Vector3 specular,
            float specularPower,
            Vector3 emissive,
            float alpha
            )
        {
            if (_sceneEffect == null) return;

            _sceneEffect.AmbientLightColor = ambient;
            _sceneEffect.DiffuseColor = diffuse;
            _sceneEffect.SpecularColor = specular;
            _sceneEffect.EmissiveColor = emissive;
            _sceneEffect.Alpha = alpha;
            _sceneEffect.SpecularPower = specularPower;
            _sceneEffect.CommitChanges();
        }

        public void SetTexture(Texture2D texture)
        {
            if (_sceneEffect == null) return;

            if (texture != null)
            {
                _sceneEffect.Texture = texture;
            }
            else
            {
                TextureMappingEnabled(false);
            }
        }

        public void TextureMappingEnabled(bool enabled)
        {
            if (_sceneEffect == null) return;
            
            _sceneEffect.TextureEnabled = enabled;
        }

        /// <summary>
        /// Sets the effects world transform property
        /// </summary>
        public void SetWorld(Matrix world)
        {
            if (_sceneEffect == null) return;

            _sceneEffect.World = world;
        }

        /// <summary>
        /// Renders the logic defined in the renderLogic function delegate.
        /// 
        /// Applys all passes of the effect to the geometry.
        /// </summary>
        public void Render(RenderFunc renderLogic)
        {
            if (_sceneEffect == null) return;

            _sceneEffect.Begin();

            foreach (EffectPass pass in _sceneEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                // Do the specific rendering.
                renderLogic(_game.GraphicsDevice, this);

                pass.End();
            }

            _sceneEffect.End();
        }

        public void RenderModel(Model model, RenderFunc renderLogic)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    _sceneEffect.View = _cameraMgr.ActiveCamera.View;
                    _sceneEffect.Projection = _cameraMgr.ActiveCamera.Projection;
                    renderLogic(_game.GraphicsDevice, this);
                    part.Effect = _sceneEffect;  
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Use to render a scene.
        /// 
        /// Will ensure that the Correct camera and viewport are used!
        /// </summary>
        public void DrawScene(RCSpatial sceneRoot)
        {
            if (_sceneEffect == null) return;
 
            bool fCameraSuccess = false;

            _sceneEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            _sceneEffect.GraphicsDevice.RenderState.DepthBufferEnable = true;

            fCameraSuccess = UpdateSceneCameraParameters();

            if (_sceneEffect != null && fCameraSuccess)
            {
                // Clear screen using current clear color.
                if (_cameraMgr.ActiveCamera.ClearScreen)
                {
                    ClearScreen();
                }
                
                sceneRoot.Draw(this);
            }
        }

        public void ClearScreen()
        {
            if (_sceneEffect == null) return;

            Graphics.Clear(
                _cameraMgr.ActiveCamera.ClearOptions,
                _clearColor,
                1.0f,
                0
                );
        }

        protected bool UpdateSceneCameraParameters()
        {
            if (_sceneEffect == null || _cameraMgr.ActiveCamera == null)
            {
                return false;
            }

            // Ensure that the correct viewport is drawn to.
            _sceneEffect.GraphicsDevice.Viewport = _cameraMgr.ActiveCamera.Viewport;
            _sceneEffect.View = _cameraMgr.ActiveCamera.View;
            _sceneEffect.Projection = _cameraMgr.ActiveCamera.Projection;

            _clearColor = _cameraMgr.ActiveCamera.ClearColor;

            return true;
        }
    }
}