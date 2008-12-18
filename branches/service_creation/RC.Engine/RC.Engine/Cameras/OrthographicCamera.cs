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
    /// Defines a camera that has a rectangular view volume.
    /// </summary>
    /// <remarks>
    /// Care must be taken when defining the with and height with respect to the viewport.
    /// The rendered view will look distorted if the aspect ratio's of the volume and viewport are different.
    /// </remarks>
    public class RCOrthographicCamera : RCCamera
    {
        private float _width;
        private float _height;

        /// <summary>
        /// Width of the view volume.
        /// </summary>
        public float Width 
        {
            get { return _width; }
            set { _width = value; }
        }
        
        /// <summary>
        /// Height of the view volume.
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value;}
        }
        
        /// <summary>
        /// Creates a camera with orthographic projection.
        /// </summary>
        /// <param name="newViewport">The region on the screen the camera's view is rendered.</param>
        public RCOrthographicCamera(Viewport newViewport)
            : base(newViewport)
        {
            _width = 1.0f;
            _height = 1.0f;
        }

        /// <summary>
        /// Creates the orthographic view volume.
        /// </summary>
        /// <returns></returns>
        protected override Matrix UpdateProjection()
        {
            return Matrix.CreateOrthographic(
                _width,
                _height,
                _near,
                _far
                );
        }
    }
}