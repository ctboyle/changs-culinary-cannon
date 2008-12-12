using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using RC.Engine.GraphicsManagement;
using RC.Engine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Input;
using RC.Engine.SceneEffects;
using RC.Engine.ContentManagement;
using RC.Engine.StateManagement;
using RC.Engine.Base;
using RC.Physics;
using RC.Engine.GraphicsManagement.BoundingVolumes;
using RC.Content.Heightmap;
using RC.Engine.ContentManagement.ContentTypes;
using JigLibX.Math;

namespace RC.Engine.Test
{
    class  GameState : RCGameState
    {
        private const int NumPlayers = 4;


        private RCSceneNode _sceneRoot = null;
        private TimeSpan _lastFrameReportTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private int _framesPerSecond = 0;
        private RCSpriteBatch _spriteBatch = null;
        private RCContent<SpriteFont> _spriteFont = null;
        private JibLibXObject physicsEnemy = null;

        private Dictionary<PlayerIndex, Player> _players = new Dictionary<PlayerIndex, Player>();

        float ang = 0.0f;


        public GameState(RCGameContext gameCtx)
            : base(gameCtx)
        {
        }

        public override void Initialize()
        {
            _sceneRoot = new RCSceneNode();

            /////////////////////////////////////////////////////////////////////
            // Setup the SpriteBatch content for displaying the FPS text
            /////////////////////////////////////////////////////////////////////
            _spriteBatch = new RCSpriteBatch();
            _spriteBatch.Enable(Ctx.Graphics);
            _spriteFont = new RCDefaultContent<SpriteFont>(Ctx.ContentRqst, "Content\\Fonts\\DefaultFont");

            for (int iPlayer = 0; iPlayer < NumPlayers; iPlayer++)
            {
                Player player = new Player((PlayerIndex)(iPlayer), NumPlayers);
                RCSpatial playerContent = player.CreatePlayerContent(Ctx);
                playerContent.LocalTrans = Matrix.Identity;


                _sceneRoot.AddChild(playerContent);

                _players[player.PlayerIndex] = player;
            }

            RCDepthBufferState depthState = new RCDepthBufferState();
            depthState.DepthTestingEnabled = true;
            _sceneRoot.GlobalStates.Add(depthState);




            /////////////////////////////////////////////////////////////////////
            // Add a controller for the camera
            /////////////////////////////////////////////////////////////////////
            
            /////////////////////////////////////////////////////////////////////
            // Create and setup the light
            /////////////////////////////////////////////////////////////////////
            
            RCLight sun = new RCLight();
            sun.Diffuse = new Vector3(1.0f);
            sun.Specular = new Vector3(1.0f);


            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 10.0f, 3.0f), Vector3.Zero, Vector3.Up);
            sun.Transform = Matrix.Invert(lightLookAt);
            _sceneRoot.AddLight(sun);

            /////////////////////////////////////////////////////////////////////
            // Create the height map
            /////////////////////////////////////////////////////////////////////
            float heightMapScaling = 10;

            RCContent<RCHeightMap> heightMap = new RCDefaultContent<RCHeightMap>(Ctx.ContentRqst, "Content\\Textures\\final_heightmap");
            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tilable_long_grass");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\seamless_rock");
            RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tileable_snow");
            heightMap.Content.Scaling = heightMapScaling;

            HeightMapEffect effect = new HeightMapEffect(Ctx.ContentRqst, heightMap,
                texture1, texture2, texture3, .3f, .55f, .65f, .75f);
            heightMap.Content.AddEffect(effect);

            // Attach the heightmap to the pysics system.
            JibLibXObject physicsHeightMap = JibLibXPhysicsHelper.CreateHeightmap(heightMap);

            _sceneRoot.UpdateRS();

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _lastFrameReportTime += gameTime.ElapsedGameTime;
            _frameCount++;

            if (_lastFrameReportTime.TotalSeconds >= 1.0f)
            {
                _framesPerSecond = _frameCount;
                _frameCount = 0;
                _lastFrameReportTime = TimeSpan.Zero;
            }


            foreach (KeyValuePair<PlayerIndex, Player> kvPlayers in _players)
            {
                Ctx.CameraMgr.SetActiveCamera(kvPlayers.Value.PlayerCameraLabel);
                Ctx.RenderMgr.Draw(_sceneRoot);
            }



            // Draw the scene statistics
            string message = string.Format("FPS: {0}\n", _framesPerSecond);

            _spriteBatch.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
            _spriteBatch.SpriteBatch.DrawString(_spriteFont, message, Vector2.Zero, Color.Yellow);
            _spriteBatch.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput();
            _sceneRoot.UpdateGS(gameTime, true);
            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Ctx.StateStack.PopState();
            }

        }
    }
}
