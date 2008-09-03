using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RC.Engine.Cameras;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Rendering
{
    public delegate void RenderFunc(GraphicsDevice device);

    public enum DirectionalLightIndex
    {
        Light0 = 0,
        Light1,
        Light2,
        Count
    }

    public interface IRCRenderManager
    {
        void Load(IServiceProvider services);

        void Unload();
        
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
        
        void Render(GraphicsDevice device, RenderFunc renderLogic);

        void RenderScene(GameTime gametime, RCSpatial sceneRoot);
    }

    /// <summary>
    /// Central functionality for Rendering the Scene.
    /// </summary>
    internal class RCRenderManager : IRCRenderManager
    {
        private BasicEffect _sceneEffect;
        private int _countEnabledLights = 0;
        private Color _clearColor = Color.CornflowerBlue;
        private IRCCameraManager _cameraMgr;
        private IServiceProvider _services;

        public RCRenderManager()
        {
            System.Console.WriteLine("Created Render Manager");
        }

        public void Load(IServiceProvider services)
        {
            _services = services;
            _cameraMgr = services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;

            IGraphicsDeviceService graphics = services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
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
            if (_sceneEffect != null)
            {
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
        }


        /// <summary>
        /// Disables the light refrenced by lightNode's index. Disables lighting if
        /// enabled light count goes to zero.
        /// </summary>
        public void DisableDirectionalLight(RCDirectionalLight lightNode)
        {
            if (_sceneEffect != null)
            {
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
            if (_sceneEffect != null)
            {
                _sceneEffect.AmbientLightColor = ambient;
                _sceneEffect.DiffuseColor = diffuse;
                _sceneEffect.SpecularColor = specular;
                _sceneEffect.EmissiveColor = emissive;
                _sceneEffect.Alpha = alpha;
                _sceneEffect.SpecularPower = specularPower;

                _sceneEffect.CommitChanges();
            }
        }

        public void SetTexture(Texture2D texture)
        {
            if (_sceneEffect != null)
            {
                if (texture != null)
                {
                    _sceneEffect.Texture = texture;
                }
                else
                {
                    TextureMappingEnabled(false);
                }
            }
            
        }

        public void TextureMappingEnabled(bool fEnabled)
        {
            if (_sceneEffect != null)
            {
                _sceneEffect.TextureEnabled = fEnabled;
            }
        }

        /// <summary>
        /// Sets the effects world transform property
        /// </summary>
        public void SetWorld(Matrix world)
        {
            if (_sceneEffect != null)
            {
                _sceneEffect.World = world;
            }
        }


        /// <summary>
        /// Renders the logic defined in the renderLogic function delegate.
        /// 
        /// Applys all passes of the effect to the geometry.
        /// </summary>
        public void Render(GraphicsDevice device, RenderFunc renderLogic)
        {   
            if (_sceneEffect != null)
            {
                _sceneEffect.Begin();

                foreach (EffectPass pass in _sceneEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    // Do the specific rendering.
                    renderLogic(device);

                    pass.End();
                }

                _sceneEffect.End();
            }
        }

        public void RenderScene(GameTime gameTime, RCSpatial sceneRoot)
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

                sceneRoot.Draw(gameTime, _services);
            }
        }

        protected void ClearScreen()
        {
            if (_sceneEffect != null)
            {

                _sceneEffect.GraphicsDevice.Clear(
                        _cameraMgr.ActiveCamera.ClearOptions,
                        _clearColor,
                        1.0f,
                        0
                        );

            }
        }

        protected bool UpdateSceneCameraParameters()
        {
            bool fUpdatedCameraParameters = false;

            if (_sceneEffect != null)
            {
                if (_cameraMgr.ActiveCamera != null)
                {
                    // Ensure that the correct viewport is drawn to.
                    _sceneEffect.GraphicsDevice.Viewport = _cameraMgr.ActiveCamera.Viewport;
                    _sceneEffect.View = _cameraMgr.ActiveCamera.View;
                    _sceneEffect.Projection = _cameraMgr.ActiveCamera.Projection;

                    _clearColor = _cameraMgr.ActiveCamera.ClearColor;

                    fUpdatedCameraParameters = true;
                }
            }

            return fUpdatedCameraParameters;
        }
    }
}