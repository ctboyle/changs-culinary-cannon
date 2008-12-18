using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;
using RC.Engine.Rendering;

namespace RC.Engine.SceneEffects
{
    /// <summary>
    /// Light effect that controls rendering of lighting in RC Engine.
    /// </summary>
    public class RCLightEffect : RCEffect
    {
        private List<RCLight> _lights = new List<RCLight>();

        public RCLightEffect()
            : base()
        {
        }

        /// <summary>
        /// Gets the maximum number of lights that can be used with this light effect.
        /// </summary>
        public int MaxLights
        {
            get { return 3; }
        }

        /// <summary>
        /// Adds lights to be rendered with theis effect.
        /// </summary>
        /// <param name="light">Light to be added with this effect.</param>
        public void AddLight(RCLight light)
        {
            if (_lights.Count >= MaxLights)
            {
                throw new InvalidOperationException("Cannot add more than maximum allowed lights.");
            }

            if (!_lights.Contains(light))
            {
                _lights.Add(light);
            }
        }

        /// <summary>
        /// Remove a light to be rendered with this effect.
        /// </summary>
        /// <param name="light"></param>
        public void RemoveLight(RCLight light)
        {
            _lights.Remove(light);
        }
        /// <summary>
        /// Removes all lights from this effect.
        /// </summary>
        public void RemoveAllLights()
        {
            _lights.Clear();
        }


        public override void CustomConfigure(IRCRenderManager render)
        {
            ((BasicEffect)Content).Projection = render.Projection;
            ((BasicEffect)Content).World = render.World;
            ((BasicEffect)Content).View = render.View;

            RCMaterialState materialState = (RCMaterialState)render.GetRenderState(RCRenderState.StateType.Material);

            ((BasicEffect)Content).DiffuseColor = materialState.Diffuse.ToVector3();
            ((BasicEffect)Content).AmbientLightColor = materialState.Ambient.ToVector3();
            ((BasicEffect)Content).SpecularColor = materialState.Specular.ToVector3();
            ((BasicEffect)Content).SpecularPower = materialState.Shininess;
            ((BasicEffect)Content).Alpha = materialState.Alpha;

            


            if (_lights.Count > 0)
            {
                ((BasicEffect)Content).LightingEnabled = true;
            }
            else
            {
                ((BasicEffect)Content).LightingEnabled = false;
            }
        
            for (int i = 0; i < 3; i++)
            {
                BasicDirectionalLight basicLight = GetBasicLight(i);

                if (_lights.Count > i)
                {
                    basicLight.Enabled = true;
                    basicLight.DiffuseColor = _lights[i].Diffuse;
                    basicLight.Direction = _lights[i].Transform.Forward;
                    basicLight.SpecularColor = _lights[i].Specular;
                }
                else
                {
                    basicLight.Enabled = false;
                }
            }
        }

        private BasicDirectionalLight GetBasicLight(int i)
        {
            BasicDirectionalLight basicLight = null;
            switch (i)
            {
                case 0:
                    basicLight = ((BasicEffect)Content).DirectionalLight0;
                    break;
                case 1:
                    basicLight = ((BasicEffect)Content).DirectionalLight1;
                    break;
                case 2:
                    basicLight = ((BasicEffect)Content).DirectionalLight2;
                    break;
                default:
                    break;
            }

            return basicLight;
        }

        protected override object OnCreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            return new BasicEffect(graphics.GraphicsDevice, null); 
        }
    }
}