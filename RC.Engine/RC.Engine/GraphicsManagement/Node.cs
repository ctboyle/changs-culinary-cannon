#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RC.Engine.GraphicsManagement.BoundingVolumes;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;
using RC.Engine.ContentManagement;
#endregion

namespace RC.Engine.GraphicsManagement
{
    /// <summary>
    /// Implements the ablity for a SceneObject to have Children.
    /// 
    /// The RCNode's bounding volume is computed by merging all of its children.
    /// </summary>
    public class RCSceneNode : RCSpatial, INode
    {
        protected List<RCSpatial> _listChildren;
        
        public RCSceneNode()
        {
            _listChildren = new List<RCSpatial>();
        }

        /// <summary>
        /// Adds a SceneObject to Children.
        /// </summary>
        public void AddChild(RCSpatial newChild)
        {
            newChild.ParentNode = this;
            _listChildren.Add(newChild);
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        /// <param name="removeChild"></param>
        /// <returns></returns>
        public bool RemoveChild(RCSpatial removeChild)
        {
            bool removed = false;
            if (removeChild != null)
            {
                removed = _listChildren.Remove(removeChild);
            }
            return removed;
        }

        /// <summary>
        /// Draws all children 
        /// </summary>
        public override void Draw(IRCRenderManager render)
        {           
            foreach (RCSpatial child in _listChildren)
            {
                child.Draw(render);
            }
        }

        /// <summary>
        /// Overriden. Updates the  node's world data.
        /// 
        /// Calls all the chilren's Update methods.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void UpdateWorldData(GameTime gameTime)
        {
            base.UpdateWorldData(gameTime);

            foreach (RCSpatial child in _listChildren)
            {
                child.UpdateGS(gameTime, false);
            }
        }

        /// <summary>
        /// Overriden. Will ensure the node's BV is the correct size.
        /// 
        /// Calls all the chilren's Update methods.
        /// </summary>
        protected override void UpdateWorldBound()
        {
            // Update the Node's bounding volume size so that it is the
            // smallest volume that can contains all the children BVs.

            Boolean fFirstChild = true;
            foreach (RCSpatial child in _listChildren)
            {    
                // Use the first child to define the initial BV.
                if (fFirstChild)
                {
                    // TODO: See if I need to use 'clone()'
                    _worldBound = child.WorldBound;
                    fFirstChild = false;
                }
                else
                {
                    // Merge the remaining children's BVs.
                    _worldBound = RCBoundingSphere.CreateMerged(
                        WorldBound,
                        child.WorldBound
                    ); 
                }
            }
        }

        /// <summary>
        /// Updates each child's render states and lights.
        /// </summary>
        /// <param name="stateStack"></param>
        /// <param name="lightStack"></param>
        protected override void  UpdateState(RCRenderStateStack stateStack, Stack<RCLight> lightStack)
        {
            foreach (RCSpatial child in _listChildren)
            {
                child.UpdateRS(stateStack, lightStack);
            }
        }

        #region INode Members

        public List<ISpatial> GetChildren()
        {
            List<ISpatial> childList = new List<ISpatial>(_listChildren.Count);

            foreach (RCSpatial child in _listChildren)
            {
                childList.Add(child);
            }

            return childList;
        }

        #endregion
    }
}


