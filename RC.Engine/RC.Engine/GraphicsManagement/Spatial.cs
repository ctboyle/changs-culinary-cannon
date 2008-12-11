#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RC.Engine.Animation;
using RC.Engine.Cameras;
using RC.Engine.GraphicsManagement.BoundingVolumes;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;

using RC.Engine.ContentManagement;

#endregion

namespace RC.Engine.GraphicsManagement
{
    /// <summary>
    /// This serves as the basic object component. It assumes that all Scene Ojbects
    /// will have:
    ///  1. Position in space (world transform)
    ///  2. A bounding Volume
    ///  3. A parent
    ///  4. A position relative to its parent
    /// </summary>
    public abstract class RCSpatial : ISpatial
    {
        protected IRCBoundingVolume _worldBound;
        private RCSpatial _parentNode;
        protected Matrix _worldTrans;
        private Matrix _localTrans;

        /// <summary>
        /// List of contollers that control any time varying quantites in this object
        /// to facilitate animations.
        /// </summary>
        protected List<IController> _animateControllers = new List<IController>();

        /// <summary>
        /// Render states to apply to this and/or children of this node.
        /// </summary>
        private RCRenderStateCollection _globalStates = new RCRenderStateCollection(false);

        /// <summary>
        /// List of effects to apply while drawing.
        /// 
        /// For RCNode,
        /// All child objects are rendered with the effect rooted
        /// at the node.
        /// 
        /// </summary>
        private List<RCEffect> _effects = new List<RCEffect>();

        /// <summary>
        /// Attached Lights in the scene graph.
        /// 
        /// If a node has a light, it is applied to all children.
        /// </summary>
        protected List<RCLight> _lights = new List<RCLight>();

        public RCSpatial ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }
        
        public IRCBoundingVolume WorldBound
        {
            get { return _worldBound; }
        }

        public List<RCEffect> Effects
        {
            get { return _effects; }
            set { _effects = value; }
        }

        public Matrix LocalTrans
        {
            get { return _localTrans; }
            set { _localTrans = value; }
        }

        public Matrix WorldTrans
        {
            get { return _worldTrans; }
            set { _worldTrans = value; }
        }

        /// <summary>
        /// Use this property to configure the renderstates associated with this node.
        /// </summary>
        public RCRenderStateCollection GlobalStates
        {
            get { return _globalStates; }
            set { _globalStates = value; }
        }


        public RCSpatial()
        {
            _localTrans = Matrix.Identity;
            _worldTrans = Matrix.Identity;
            ParentNode = null;

            _worldBound = new RCBoundingSphere(
                Vector3.Zero,
                0.0f
                );
        }

        /// <summary>
        /// Called to update the SceneObject
        /// 
        /// GS stands for Graphic State
        /// </summary>
        public virtual void UpdateGS(GameTime gameTime, Boolean fInitiator)
        {
            UpdateWorldData(gameTime);
            UpdateWorldBound();
            if (fInitiator)
            {
                PropigateBVToRoot();
            }
        }

        public void UpdateRS()
        {
            UpdateRS(null, null);
        }

        public void UpdateRS(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            bool fInitiator = (stateStack == null);

            if (fInitiator)
            {
                stateStack = new RCRenderStateStack();
                lightStack = new Stack<RCLight>();

                // Ensure that states are accumulated from parents just in case we are not
                // at the graph root.
                PropigateStateFromRoot(stateStack, lightStack);
            }
            else
            {
                PushState(stateStack, lightStack);
            }

            // Manage derived-class specific state management.
            UpdateState(stateStack, lightStack);

            if (!fInitiator)
            {
                PopState(stateStack, lightStack);
            }
        }

        protected abstract void UpdateState(RCRenderStateStack stateStack, Stack<RCLight> lightStack);

        protected void PropigateStateFromRoot(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            if (_parentNode != null)
            {
                _parentNode.PropigateStateFromRoot(stateStack, lightStack);
            }

            PushState(stateStack, lightStack);
        }

        /// <summary>
        /// Places render states on a stack as the scene graph is traversed.
        /// 
        /// Also manages active lights on a lights stack.
        /// </summary>
        /// <param name="stateStack"></param>
        /// <param name="lightStack"></param>
        private void PushState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            stateStack.PushStates(_globalStates);
            
            foreach (RCLight light in _lights)
            {
                lightStack.Push(light);
            }
        }

        private void PopState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            stateStack.PopStates(_globalStates);
            
            int iSize = _lights.Count;
            for (int i = 0; i < iSize; iSize++)
            {
                lightStack.Pop();
            }
        }

        /// <summary>
        /// Override for specific behavior on the draw pass.
        /// </summary>
        public abstract void Draw(IRCRenderManager render, IRCContentRequester contentRqst);

        public bool AddController(IController controller)
        {
            bool fAttachSucceeded = false;

            if (controller != null)
            {
                _animateControllers.Add(controller);
                fAttachSucceeded = true;
            }

            return fAttachSucceeded;
        }

        public IController GetController<ContrllerType> ()
        {

            return _animateControllers.FindLast(new Predicate<IController>(
                    delegate(IController x)
                    {
                        if (x is ContrllerType)
                        {
                            return true;
                        }

                        return false;
                    }
                ));
        }   

        public void RemoveController(IController controller)
        {
            if (controller != null)
            {
                _animateControllers.Remove(controller);
            }
        }

        protected void UpdateControllers(GameTime gameTime)
        {
            for (int iController = 0; iController < _animateControllers.Count; iController++ )
            {
                _animateControllers[iController].Update(gameTime);
            }
        }

        /// <summary>
        /// Adds a light at this node to affect the rendering of this node and children.
        /// </summary>
        /// <param name="light">The light to affect this node and its children.</param>
        public void AddLight(RCLight light)
        {
            // Do not fail if light is already in list.
            if (!_lights.Contains(light))
            {
                _lights.Add(light);
            }
        }

        public void RemoveLight(RCLight light)
        {
            _lights.Remove(light);
        }

        public void AddEffect(RCEffect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException();
            }

            if (!_effects.Contains(effect))
            {
                _effects.Add(effect);
            }
        }

        public void RemoveEffect(RCEffect effect)
        {
            _effects.Remove(effect);
        }

        /// <summary>
        /// Override to update all object world oriented data.
        /// </summary>
        protected virtual void UpdateWorldData(GameTime gameTime)
        {
            // Update animations
            UpdateControllers(gameTime);


            if (ParentNode != null)
            {
                // Compute world transform from parent's and local transforms.
                _worldTrans = _localTrans * ParentNode.WorldTrans;
            }
            else
            {
                // This is root, local and world trans are identical.
                _worldTrans = _localTrans;
            }
        }
        /// <summary>
        /// Override to specify the world bounding volume for your objects
        /// </summary>
        protected abstract void UpdateWorldBound();

        /// <summary>
        /// If the SceneObject moves and its world BV changes, all its 
        /// parent node's BVs need to be updated.
        ///</summary>
        protected void PropigateBVToRoot()
        {
            if (ParentNode != null)
            {
                ParentNode.UpdateWorldBound();
                ParentNode.PropigateBVToRoot();
            }
        }
    }
}