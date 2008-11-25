using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Cameras;
using RC.Engine.GraphicsManagement;

using Ninject.Core;

namespace RC.Engine.Rendering
{
    public delegate void RenderFunc(IRCRenderManager render);

    public enum DirectionalLightIndex
    {
        Light0 = 0,
        Light1,
        Light2,
        Count
    }

    /// <summary>
    /// I am the render manager and allow rendering of geometry and scenes.
    /// </summary>
    public interface IRCRenderManager
    {
        /// <summary>
        /// The current world transformation.
        /// </summary>
        Matrix World { get; }

        /// <summary>
        /// The current view transformation.
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// The current projection transformation.
        /// </summary>
        Matrix Projection { get; }

        /// <summary>
        /// I set the render state.
        /// </summary>
        /// <param name="renderState">The render state.</param>
        void SetRenderState(RCRenderState renderState);

        /// <summary>
        /// I get the render state for a render state type.
        /// </summary>
        /// <param name="type">The render state type.</param>
        RCRenderState GetRenderState(RCRenderState.StateType type);

        /// <summary>
        /// I draw a geometric object to the screen.
        /// </summary>
        /// <param name="geometry">The geometric object.</param>
        void Draw(RCGeometry geometry);

        /// <summary>
        /// I draw a scene to the screen.
        /// </summary>
        /// <param name="sceneRoot">The scene root.</param>
        void DrawScene(RCSpatial sceneRoot);

        /// <summary>
        /// I clear the screen.
        /// </summary>
        void ClearScreen();
    }

    /// <summary>
    /// Central functionality for Rendering the Scene.
    /// </summary>
    [Singleton]
    internal class RCRenderManager : IRCRenderManager
    {
        private IGraphicsDeviceService _graphics = null;
        private IRCCameraManager _cameraMgr = null;
        private Matrix _world = Matrix.Identity;
        private RCGeometry _geometry = null;
        private RCRenderStateCollection _renderStates = 
            new RCRenderStateCollection(true);

        public Matrix World
        {
            get { return _world; }
        }

        public Matrix View
        {
            get { return CameraMgr.ActiveCamera.View; }
        }

        public Matrix Projection
        {
            get { return CameraMgr.ActiveCamera.Projection; }
        }

        public void SetRenderState(RCRenderStateCollection renderStates)
        {
            foreach (RCRenderState state in renderStates)
            {
                SetRenderState(state);
            }
        }

        public void RestoreRenderState(RCRenderStateCollection renderStates)
        {
            foreach (RCRenderState state in renderStates)
            {
                SetRenderState(RCRenderState.Default[state.GetStateType()]);
            }
        }

        public void SetRenderState(RCRenderState renderState)
        {
            if (renderState != null)
            {
                // Cache the new state
                _renderStates[renderState.GetStateType()] = renderState;

                // Enable the state
                renderState.ConfigureDevice(Graphics.GraphicsDevice);
            }
        }

        public RCRenderState GetRenderState(RCRenderState.StateType type)
        {
            return _renderStates[type];
        }

        /// <summary>
        /// Use to render a scene.
        /// 
        /// Will ensure that the Correct camera and viewport are used!
        /// </summary>
        public void DrawScene(RCSpatial sceneRoot)
        {
            if (CameraMgr.ActiveCamera == null)
            {
                throw new InvalidOperationException("Active camera must be set before drawing scene");
            }

            UpdateSceneCameraParameters();
            
            // Clear screen using current clear color.
            if (CameraMgr.ActiveCamera.ClearScreen)
            {
                ClearScreen();
            }
             
            sceneRoot.Draw(this);
        }

        public void ClearScreen()
        {
            Graphics.GraphicsDevice.Clear(
                CameraMgr.ActiveCamera.ClearOptions,
                CameraMgr.ActiveCamera.ClearColor,
                1.0f,
                0
                );
        }

        public void Draw(RCGeometry geometry)
        {
            _geometry = geometry;

            // Enable the geometry's renderstates
            SetRenderState(geometry.RenderStates);

            _world = geometry.WorldTrans;

            // Render the geometry obejct with each of the effects.
            bool isPrimaryEffect = true;
            foreach (RCEffect effect in geometry.Effects)
            {
                ApplyEffect(effect, isPrimaryEffect);
                isPrimaryEffect = false;
            }
        }


        [Inject]
        public IRCCameraManager CameraMgr
        {
            get { return _cameraMgr; }
            set { _cameraMgr = value; }
        }

        [Inject]
        public IGraphicsDeviceService Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        protected bool UpdateSceneCameraParameters()
        {
            // Ensure that the correct viewport is drawn to.
            Graphics.GraphicsDevice.Viewport = CameraMgr.ActiveCamera.Viewport;
            return true;
        }

        private void ApplyEffect(RCEffect rcEffect, bool isPrimaryEffect)
        {
            Effect shader = rcEffect.Content;

            // Configure The Effect
            rcEffect.CustomConfigure(this);

            shader.Begin();

            EffectPassCollection passes = shader.CurrentTechnique.Passes;
            for (int iPass = 0; iPass < passes.Count; iPass++ )
            {
                rcEffect.SetRenderState(iPass, this, isPrimaryEffect);

                passes[iPass].Begin();

                DrawElements();

                passes[iPass].End();

                rcEffect.RestoreRenderState(iPass, this, isPrimaryEffect);
            }

            shader.End();
        }

        private void DrawElements()
        {
            RCVertexBuffer VBuffer = _geometry.VBuffer;
            RCIndexBuffer IBuffer = _geometry.IBuffer;

            Graphics.GraphicsDevice.VertexDeclaration = VBuffer.VertexDeclaration;

            Graphics.GraphicsDevice.Vertices[0].SetSource(
                VBuffer.VertexBuffer,
                0,
                VBuffer.VertexSize
                );

            Graphics.GraphicsDevice.Indices = IBuffer.IndexBuffer;
            
            // Finally draw the actual triangles on the screen
            Graphics.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0, 0,
                VBuffer.NumVertices,
                0,
                IBuffer.NumPrimitives
                );
        }
    }
}