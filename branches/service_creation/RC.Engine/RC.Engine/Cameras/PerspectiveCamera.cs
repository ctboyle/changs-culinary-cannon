using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RC.Engine.GraphicsManagement;
using RC.Engine.Rendering;

namespace RC.Engine.Cameras
{
    /// <summary>
    /// Defines a camera with a frustum view volume.
    /// </summary>
    public class RCPerspectiveCamera : RCCamera
    {
        protected float _FOV;

        /// <summary>
        /// Gets or sets the feild of view.
        /// </summary>
        public float FOV
        {
            get { return _FOV; }
            set
            {
                _FOV = value;
                UpdateWorldData(null);
            }
        }

        /// <summary>
        /// Creates a camera with perspective projection.
        /// </summary>
        /// <param name="newViewport">The region on the screen the camera's view is rendered.</param>
        public RCPerspectiveCamera(Viewport newViewport)
            : base(newViewport)
        {
            _FOV = MathHelper.PiOver4;
  
        }

        /// <summary>
        /// Creates the pespective projection matrix.
        /// </summary>
        /// <returns></returns>
        protected override Matrix UpdateProjection()
        {
            // Create perpective projection Matrix based on viewport
            return Matrix.CreatePerspectiveFieldOfView(
               _FOV,
               (float)_viewport.Width / (float)_viewport.Height,
               _near,
               _far
               );
        }
    }
}