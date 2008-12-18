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
    /// This serves as the base scene graph object.
    /// </summary>
    /// <remarks>
    /// A spatial object by itself is a leaf in the scene graph.
    /// 
    /// As the base for all other nodes in the graph, spatial provides
    /// support for setting effects, render states, transforms, and bounding
    /// volumes at every node in the tree.
    /// </remarks>
    public abstract class RCSpatial : ISpatial
    {
        /// <summary>
        /// The computed world bound for this object.
        /// </summary>
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

        /// <summary>
        /// The parent scene graph object of this object. Is null if this is the root.
        /// </summary>
        public RCSpatial ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }
        
        /// <summary>
        /// Gets the current bounding volume in worldspace for theis object.
        /// </summary>
        public IRCBoundingVolume WorldBound
        {
            get { return _worldBound; }
        }

        /// <summary>
        /// Gets and sets the list of effects to applies to this object on the draw pass.
        /// </summary>
        public List<RCEffect> Effects
        {
            get { return _effects; }
            set { _effects = value; }
        }

        /// <summary>
        /// Gets or sets the transformation from this node's parent.
        /// If there is no parent, the local is the same as the world transform.
        /// </summary>
        public Matrix LocalTrans
        {
            get { return _localTrans; }
            set { _localTrans = value; }
        }

        /// <summary>
        /// Gets or sets the world transform for this node.
        /// </summary>
        /// <remarks>The world transform is the object's tranform in world space.</remarks>
        public Matrix WorldTrans
        {
            get { return _worldTrans; }
            set { _worldTrans = value; }
        }

        /// <summary>
        /// Use this property to configure the active renderstates associated with this node.
        /// </summary>
        /// <remarks> There are no states in the collection by default.</remarks>
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
        /// Starts an update pass from this node.
        /// </summary>
        /// <remarks>May be called from anywhere in the scene graph. Updates transforms, animations, and bounding volumes all the way to the root.</remarks>
        /// <param name="gameTime">Elasped time</param>
        /// <param name="fInitiator">Indicates whether this call is the start of an update pass. Is false in recursive calls.</param>
        public virtual void UpdateGS(GameTime gameTime, Boolean fInitiator)
        {
            UpdateWorldData(gameTime);
            UpdateWorldBound();
            if (fInitiator)
            {
                PropigateBVToRoot();
            }
        }

        /// <summary>
        /// Initiates the start of a render state update. Accumulates render states down to the leaf nodes and updates lights.
        /// </summary>
        public void UpdateRS()
        {
            UpdateRS(null, null);
        }

        /// <summary>
        /// Recursivly called to update objects render state.
        /// </summary>
        /// <param name="stateStack">Current accumulated state stack. Should be null if pass is initiated.</param>
        /// <param name="lightStack">Current accumulated light stack. Should be null if pass is initiated.</param>
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

        /// <summary>
        /// Updates the render state for derived object implementations. Called by UpdateRS during state update pass.
        /// </summary>
        /// <param name="stateStack"></param>
        /// <param name="lightStack"></param>
        protected abstract void UpdateState(RCRenderStateStack stateStack, Stack<RCLight> lightStack);

        /// <summary>
        /// Ensures that if a pass was initiated not from the root, that the current node has the correct states accumulated so far.
        /// </summary>
        /// <param name="stateStack">Current accumulated state stack.</param>
        /// <param name="lightStack">Current accumulated light stack.</param>
        protected void PropigateStateFromRoot(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            if (_parentNode != null)
            {
                _parentNode.PropigateStateFromRoot(stateStack, lightStack);
            }

            PushState(stateStack, lightStack);
        }

        /// <summary>
        /// Places render states and lights on stacks as the scene graph is traversed down to the leaves.
        /// </summary>
        /// <param name="stateStack">Current accumulated state stack.</param>
        /// <param name="lightStack">Current accumulated light stack.</param>
        private void PushState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            stateStack.PushStates(_globalStates);
            
            foreach (RCLight light in _lights)
            {
                lightStack.Push(light);
            }
        }

        /// <summary>
        /// Removes the render states and lights as the scen graph is traveresed up to the root.
        /// </summary>
        /// <param name="stateStack">Current accumulated state stack.</param>
        /// <param name="lightStack">Current accumulated light stack.</param>
        private void PopState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            stateStack.PopStates(_globalStates);
            
            int iSize = _lights.Count;
            for (int i = 0; i < iSize; i++)
            {
                lightStack.Pop();
            }
        }

        /// <summary>
        /// Override for specific behavior on the draw pass.
        /// </summary>
        public abstract void Draw(IRCRenderManager render);

        /// <summary>
        /// Sets the animation controller to be updated on this object.
        /// </summary>
        /// <remarks>Please use AnimationController.AttachToObject instead.</remarks>
        /// <param name="controller">The conntroller to add.</param>
        /// <returns>If update succeeded</returns>
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

        /// <summary>
        /// Gets the controller in the controller list based on its type.
        /// </summary>
        /// <typeparam name="ContrllerType"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Removes a controller from the controller list.
        /// </summary>
        /// <param name="controller">The controller refrence to remove.</param>
        public void RemoveController(IController controller)
        {
            if (controller != null)
            {
                _animateControllers.Remove(controller);
            }
        }

        /// <summary>
        /// Updates animations of any controller attached to this object.
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Removes a light from the lights acting on this object.
        /// </summary>
        /// <param name="light">The light reference to remove.</param>
        public void RemoveLight(RCLight light)
        {
            _lights.Remove(light);
        }

        /// <summary>
        /// Adds an effect to be rendered during the drawing of this object.
        /// </summary>
        /// <param name="effect"></param>
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

        /// <summary>
        /// Removes the effect from the list of effect to be rendered for this object.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        public void RemoveEffect(RCEffect effect)
        {
            _effects.Remove(effect);
        }

        /// <summary>
        /// Override to update all object world space data.
        /// </summary>
        /// <remarks>By default, this function updates animations and transformations.</remarks>
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