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
    public class RCGeometry : RCSpatial
    {
        public RCRenderStateCollection RenderStates = new RCRenderStateCollection(false);

        private RCLightEffect _lightEffect = null;
        private RCVertexRefrence _vertexRefrence;

        private RCBoundingSphere _localBound = new RCBoundingSphere();

        public RCBoundingSphere LocalBound
        {
            get { return _localBound; }
            set { _localBound = value; }
        }

        public RCVertexRefrence PartData
        {
            get { return _vertexRefrence; }
            set { _vertexRefrence = value; }
        }

        public RCGeometry()
        {
            
        }
        
        public override void Draw(IRCRenderManager render, IRCContentRequester contentRqst)
        {

            if (_lightEffect != null && !_lightEffect.IsInitialized)
            {
                _lightEffect.Initialize(contentRqst);
            }
            
            render.Draw(this);
        }

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

        protected override void UpdateWorldBound()
        {
            _worldBound = _localBound.Transform(WorldTrans);
        }
    }
}