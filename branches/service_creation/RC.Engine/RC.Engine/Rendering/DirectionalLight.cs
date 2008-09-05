using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RC.Engine.GraphicsManagement;

namespace RC.Engine.Rendering
{
    /// <summary>
    /// Represents a Directional light.
    /// 
    /// Children of this node will we rendered with this light enabled.
    /// </summary>
    public class RCDirectionalLight : RCSceneNode
    {
        protected RCLightSource _lightSource;
        protected DirectionalLightIndex _index;
        protected Vector3 _direction;
        protected Vector3 _diffuse;
        protected Vector3 _specular;

        public DirectionalLightIndex LightIndex
        {
            get
            {
                return _index;
            }
        }
        
        public Vector3 Direction
        {
            get
            {
                return _lightSource.WorldTrans.Forward;
            }
        }

        public Vector3 Diffuse
        {
            get
            {
                return _diffuse;
            }
            set
            {
                _diffuse = value;
            }
        }

        public Vector3 Specular
        {
            get
            {
                return _specular;
            }
            set
            {
                _specular = value;
            }
        }

        public RCLightSource LightSource
        {
            get { return _lightSource; }
        }
        
        public RCDirectionalLight(DirectionalLightIndex index)
        {
            _lightSource = new RCLightSource(this);
            _index = index;
        }

        public override void Draw(IRCRenderManager render)
        {
            // Enable light before rendering children
            render.EnableDirectionalLight(this);
            
            // Draw children.
            base.Draw(render);

            // disable light before continuing drawing in the scene graph.
            render.DisableDirectionalLight(this);
        }
    }
}
