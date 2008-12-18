using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using RC.Engine.SceneEffects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.ContentManagement;
using RC.Engine.GraphicsManagement.BoundingVolumes;

namespace RC.Engine.GraphicsManagement
{
    /// <summary>
    /// Geometry is a leaf object in the scen graph that has renderable data.
    /// </summary>
    public class RCGeometry : RCSpatial
    {
        /// <summary>
        /// The list of accumulated render states this object should be rendered with.
        /// </summary>
        public RCRenderStateCollection RenderStates = new RCRenderStateCollection(false);

        private RCLightEffect _lightEffect = null;
        private RCVertexRefrence _vertexRefrence;

        private RCBoundingSphere _localBound = new RCBoundingSphere();

        /// <summary>
        /// The bounding volume in object space for this object.
        /// </summary>
        public RCBoundingSphere LocalBound
        {
            get { return _localBound; }
            set { _localBound = value; }
        }

        /// <summary>
        /// The vertex reference to be used to render this model's data.
        /// </summary>
        public RCVertexRefrence PartData
        {
            get { return _vertexRefrence; }
            set { _vertexRefrence = value; }
        }

        public RCGeometry()
        {
            
        }
        
        /// <summary>
        /// Draws the geometric data as a triangle list.
        /// </summary>
        /// <param name="render">The render manager</param>
        /// <param name="contentRqst">The content requester for on demand loading</param>
        public override void Draw(IRCRenderManager render)
        {
            render.Draw(this);
        }

        /// <summary>
        /// Sets the accumulated renderstates and lights to be used for the drawing o fthis object.
        /// </summary>
        /// <param name="stateStack"></param>
        /// <param name="lightStack"></param>
        protected override void UpdateState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            foreach (RCRenderState.StateType type in Enum.GetValues(typeof(RCRenderState.StateType)))
            {
                RenderStates.Add(stateStack[type]);
            }

            // If lights, add lighting effect and update them.
            if (lightStack.Count > 0)
            {
                if (_lightEffect != null)
                {
                    _lightEffect.RemoveAllLights();
                }
                else
                {
                    // Create a new light effect and put it to be rendered first in the list.
                    _lightEffect = new RCLightEffect();
                    Effects.Insert(0, _lightEffect);
                }
                // Make sure the

                foreach (RCLight light in lightStack)
                {
                    _lightEffect.AddLight(light);
                }
            }
            else
            {
                if (_lightEffect != null)
                {
                    // No lights, remove it from the list.
                    Effects.Remove(_lightEffect);
                    _lightEffect = null; // TODO: See about keeping the refrence to avoid GC
                }
            }
        }

        /// <summary>
        /// Tansforms the local bounding volume to the world bounding volume.
        /// </summary>
        protected override void UpdateWorldBound()
        {
            _worldBound = _localBound.Transform(WorldTrans);
        }
    }
}
