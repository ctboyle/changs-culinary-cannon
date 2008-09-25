using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.SceneEffects
{
    public class RCLightSource : RCSceneNode
    {
        RCLightNode _lightNode;
        IRCLight _light;

        public RCLightNode LightNode
        {
            get { return _lightNode; }
        }

        public IRCLight Light
        {
            get
            {
                return _light;
            }

            set
            {
                _light = value;
                if (LightNode.Light != _light)
                {
                    LightNode.Light = _light;
                }

            }
        }

        public RCLightSource(RCLightNode lightNode)
            : base()
        {
            _lightNode = lightNode;
        }

        protected override void UpdateWorldData(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.UpdateWorldData(gameTime);

            _light.Direction = WorldTrans.Forward;
            _light.Position = WorldTrans.Translation;
        }
    }
}
