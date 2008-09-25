using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RC.Engine.SceneEffects
{
    public class RCLightEffect : RCEffect
    {
        List<RCLight> _lights = new List<RCLight>();

        public RCLightEffect() { }

            


        public int MaxLights
        {
            get { return 3; }
        }

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

        public void RemoveLight(RCLight light)
        {
            _lights.Remove(light);
        }

        public void RemoveAllLights()
        {
            _lights.Clear();
        }

        public override void CustomConfigure(IRCRenderManager render)
        {
            ((BasicEffect)Effect).Projection = render.Projection;
            ((BasicEffect)Effect).World = render.World;
            ((BasicEffect)Effect).View = render.View;

            RCMaterialState materialState = (RCMaterialState)render.GetRenderState(RCRenderState.StateType.MATERIAL);

            ((BasicEffect)Effect).DiffuseColor = materialState.Diffuse.ToVector3();
            ((BasicEffect)Effect).AmbientLightColor = materialState.Ambient.ToVector3();
            ((BasicEffect)Effect).SpecularColor = materialState.Specular.ToVector3();
            ((BasicEffect)Effect).SpecularPower = materialState.Shininess;
            ((BasicEffect)Effect).Alpha = materialState.Alpha;


            if (_lights.Count > 0)
            {
                ((BasicEffect)Effect).LightingEnabled = true;
            }
            else
            {
                ((BasicEffect)Effect).LightingEnabled = false;
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
                    basicLight = ((BasicEffect)Effect).DirectionalLight0;
                    break;
                case 1:
                    basicLight = ((BasicEffect)Effect).DirectionalLight1;
                    break;
                case 2:
                    basicLight = ((BasicEffect)Effect).DirectionalLight2;
                    break;
                default:
                    break;
            }

            return basicLight;
        }

        protected override Effect LoadEffect(GraphicsDevice myDevice, ContentManager myLoader)
        {
            return new BasicEffect(myDevice, null);
        }

    }
}
