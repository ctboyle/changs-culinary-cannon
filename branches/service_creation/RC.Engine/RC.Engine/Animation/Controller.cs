using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Animation
{
    /// <summary>
    /// Defines a controller class that updates a <see cref="RCSpatial"/>.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Returns if the controller is animating.
        /// </summary>
        bool IsAnimating { get; }

        /// <summary>
        /// Called during an update pass.
        /// </summary>
        /// <param name="gameTime">The current gametime.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// The spatial object that the controller controls.
        /// </summary>
        RCSpatial Parent { get;}
    }

    /// <summary>
    /// Abstract instance of the <see cref="IController"/> interface.
    /// </summary>
    /// <typeparam name="CntrlType">The actual <see cref="RCSpatial"/> type.</typeparam>
    public abstract class Controller<CntrlType> : IController where CntrlType : RCSpatial
    {
        /// <summary>
        /// The spatial object that is controlled.
        /// </summary>
        protected CntrlType _controlledItem;

        /// <summary>
        /// Flag that determines if the controller is animating.
        /// </summary>
        protected bool _isAnimating;

        /// <summary>
        /// The spatial object that the controller controls.
        /// </summary>
        public RCSpatial Parent
        {
            get { return _controlledItem; }
        }

        /// <summary>
        /// Returns if the controller is animating.
        /// </summary>
        public bool IsAnimating
        {
            get { return _isAnimating; }
        }

        /// <summary>
        /// Attaches
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool AttachToObject(CntrlType parent)
        {
            bool fSuccess = false;
            if (parent != null)
            {
                _controlledItem = parent;
                fSuccess = _controlledItem.AddController(this);
            }

            return fSuccess;
        }

        /// <summary>
        /// Creates a new instance of the controller.
        /// </summary>
        public Controller()
        {
            _controlledItem = null;
            _isAnimating = false;
        }

        /// <summary>
        /// Called during an update pass.
        /// </summary>
        /// <param name="gameTime">The current gametime.</param>
        public abstract void Update(GameTime gameTime);
    }
}
