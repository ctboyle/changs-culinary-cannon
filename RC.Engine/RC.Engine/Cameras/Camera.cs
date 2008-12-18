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
    /// Camera class is a RCSceneNode that defines the way a scene is viewed.
    /// </summary>
    /// <remarks>
    /// A camera is used by providing a viewport and specifing camera proprties.
    /// After creating a camera, it should be added to the camera manger in order fir it
    /// to be used
    /// </remarks>
    public abstract class RCCamera : RCSceneNode
    {
        protected Matrix _view;
        protected Matrix _projection;
        protected Viewport _viewport;
        protected Color _clearColor;
        protected float _near;
        protected float _far;

        private bool _clearScreen;
        private ClearOptions _clearOptions;

        /// <summary>
        ///  Specifies which buffer to use when using this camera to clear the screen.
        /// </summary>
        public ClearOptions ClearOptions
        {
            get { return _clearOptions; }
            set { _clearOptions = value; }
        }

        /// <summary>
        /// Indicates weather the camera should clear the screen when used as the active camera.
        /// </summary>
        public bool ClearScreen
        {
            get { return _clearScreen; }
            set { _clearScreen = value; }
        }
         
        /// <summary>
        /// Gets the computed view matrix for this camera.
        /// </summary>
        public Matrix View
        {
            get { return _view; }
        }

        /// <summary>
        /// Gets the projection matrix associated with this camera.
        /// </summary>
        public Matrix Projection
        {
            get { return _projection; }
        }

        /// <summary>
        /// Gets the viewport of the screen the camera view will be drawn.
        /// </summary>
        public Viewport Viewport
        {
            get { return _viewport; }

        }


        /// <summary>
        /// Gets or sets the near plane distance.
        /// </summary>
        public float Near
        {
            get { return _near; }
            set { _near = value; }
        }

        /// <summary>
        ///  Gets or sets the far palne distance.
        /// </summary>
        public float Far
        {
            get { return _far; }
            set { _far = value; }
        }
        
        /// <summary>
        /// Specifices the color the camera should use to clear the screen.
        /// </summary>
        public Color ClearColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        /// <summary>
        ///  Creates a new camera using the viewport specified.
        /// </summary>
        /// <param name="newViewport">The region on the screen the camera's view is rendered.</param>
        public RCCamera(Viewport newViewport)
            : base()
        {
            _viewport = newViewport;
            SetDefaultClipDepth();


            _clearScreen = true;
            _clearOptions = 
                ClearOptions.DepthBuffer |
                ClearOptions.Target;

            _near = 1.0f;
            _far = 1000.0f;
            

            ClearColor = Color.CornflowerBlue;
        }

        private void SetDefaultClipDepth()
        {
            _viewport.MinDepth = 0.0f;
			_viewport.MaxDepth = 1.0f;
        }

        /// <summary>
        /// Gets a ray in world space that is defined by a set of screen corrdinates extending from the camera origin.
        /// </summary>
        /// <param name="screenCoords">The screen cordinates to generate the ray.</param>
        /// <returns> The computed ray.</returns>
        public Ray? UnprojectWorldRay(Point screenCoords)
        {
            Ray ray;
            
            Vector3 near = Viewport.Unproject(
                new Vector3(
                    screenCoords.X,
                    screenCoords.Y,
                    0.0f
                ),
                _projection,
                _view,
                Matrix.Identity
                );

            Vector3 far = Viewport.Unproject(
                new Vector3(
                    screenCoords.X,
                    screenCoords.Y,
                    1.0f
                ),
                _projection,
                _view,
                Matrix.Identity
                );

            Vector3 direction = far - near;
            direction.Normalize();

            ray = new Ray(
                near, 
                direction
                );
            

            return ray;
            
        }

        /// <summary>
        /// Determines whether the screen cordinates given residfe in the camera's viewport.
        /// </summary>
        /// <param name="screenCoords">Screen cordinates to test.</param>
        /// <returns>True if screen cordinates are in the viewport.</returns>

        public bool ContainsPoint(Point screenCoords)
        {
            bool contains = false;
            
            contains =  (screenCoords.X >= Viewport.X) &&
                        (screenCoords.Y >= Viewport.Y) &&
                        (screenCoords.X < Viewport.X + Viewport.Width) &&
                        (screenCoords.Y < Viewport.Y + Viewport.Height);
            
            return contains;
        }

        /// <summary>
        /// Will update the view and projection matricies along with the default base implementaion.
        /// </summary>
        /// <param name="gameTime">Elasped time</param>
        protected override void UpdateWorldData(GameTime gameTime)
        {
            base.UpdateWorldData(gameTime);

            _view = UpdateView();

            _projection = UpdateProjection();

        }

        private Matrix UpdateView()
        {
            // Create view matrix from world tranform.
            return Matrix.Invert(WorldTrans);
        }

        /// <summary>
        /// Determins the projection matrix for the camera.
        /// </summary>
        /// <returns></returns>
        protected abstract Matrix UpdateProjection();
    }
}