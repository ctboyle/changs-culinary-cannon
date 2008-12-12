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
using JigLibX.Collision;

namespace RC.Engine.Test
{
    class GameState : RCGameState
    {
        private const int NumPlayers = 4;


        private RCSceneNode _sceneRoot = null;
        private TimeSpan _lastFrameReportTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private int _framesPerSecond = 0;
        private RCSpriteBatch _spriteBatch = null;
        private RCContent<SpriteFont> _spriteFont = null;
        private JibLibXObject physicsEnemy = null;
        private RCLevelCollection levels;
        public List<RCLevelSpawnPoint> spawnPoints
        {
            get
            {
                return levels.ActiveLevel.SpawnPoints;
            }
        }

        private Dictionary<PlayerIndex, Player> _players = new Dictionary<PlayerIndex, Player>();

        float ang = 0.0f;


        public GameState(RCGameContext gameCtx)
            : base(gameCtx)
        {
        }

        public override void Initialize()
        {
            _sceneRoot = new RCSceneNode();

            Ctx.Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            /////////////////////////////////////////////////////////////////////
            // Setup the SpriteBatch content for displaying the FPS text
            /////////////////////////////////////////////////////////////////////
            _spriteBatch = new RCSpriteBatch();
            _spriteBatch.Enable(Ctx.Graphics);
            _spriteFont = new RCDefaultContent<SpriteFont>(Ctx.ContentRqst, "Content\\Fonts\\DefaultFont");

            levels = new RCLevelCollection(Ctx.Graphics);
            SetUpLevels(_sceneRoot);

            PotatoPool pool = new PotatoPool(Ctx.ContentRqst, 10, _sceneRoot);
            for (int iPlayer = 0; iPlayer < NumPlayers; iPlayer++)
            {
                Player player = new Player((PlayerIndex)(iPlayer),pool, NumPlayers);
                player.CreatePlayerContent(_sceneRoot, Ctx);
                player.SetPlayerPosition(levels.ActiveLevel.SpawnPoints[iPlayer]);

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
            string message = string.Format("FPS: {0}\nPosition: {1}\nFacing: {2} \n", _framesPerSecond, Ctx.CameraMgr.ActiveCamera.WorldTrans.Translation, Ctx.CameraMgr.ActiveCamera.WorldTrans.Forward);

            _spriteBatch.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
            _spriteBatch.SpriteBatch.DrawString(_spriteFont, message, Vector2.Zero, Color.Yellow);
            _spriteBatch.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            
            _sceneRoot.UpdateGS(gameTime, true);
            UpdateInput(gameTime);
             
            base.Update(gameTime);
        }

        private void UpdateInput(GameTime gameTime)
        {
            foreach (KeyValuePair<PlayerIndex, Player> kvPlayers in _players)
            {
                GamePadState padState = GamePad.GetState(kvPlayers.Key);
                kvPlayers.Value.UpdateInput(gameTime, padState);

            } 
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Ctx.StateStack.PopState();
            }

        }

        private void SetUpLevels(RCSceneNode sceneNode)
        {
            #region level 1
            {
                List<RCLevelSpawnPoint> level1SpawnPoints = new List<RCLevelSpawnPoint>();
                level1SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.912f, +0.535f, -0.302f), new Vector3(-0.045f, -0.029f, +0.084f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.263f, +0.502f, -0.031f), new Vector3(-0.098f, -0.013f, +0.008f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.638f, +0.430f, +0.480f), new Vector3(+0.057f, -0.021f, -0.079f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.209f, +0.441f, +0.917f), new Vector3(+0.003f, -0.005f, -0.100f)));

                RCLevelParameters level1Params = new RCLevelParameters(Ctx.ContentRqst, "Deathmatch Level 1",
                    "tilable_long_grass", "seamless_rock", "tileable_snow", 450f, 1, .3f, .55f, .65f, .75f,
                    sceneNode, level1SpawnPoints, "Grassy Canyon Battle",100,15,5);

                levels.Add(new RCLevel(level1Params,levels));
            }
            #endregion level 1

            #region level 2
            {
                List<RCLevelSpawnPoint> level2SpawnPoints = new List<RCLevelSpawnPoint>();
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.554f, +0.523f, -0.646f), new Vector3(+0.688f, +0.025f, +0.725f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.554f, +0.512f, -0.023f), new Vector3(+0.999f, +0.025f, -0.026f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.079f, +0.525f, +0.571f), new Vector3(+0.078f, +0.025f, -0.997f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.585f, +0.537f, +0.581f), new Vector3(+0.743f, +0.025f, -0.669f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.614f, +0.551f, +0.555f), new Vector3(-0.758f, -0.080f, -0.647f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.702f, +0.564f, -0.698f), new Vector3(-0.994f, -0.028f, -0.104f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.650f, +0.556f, -0.626f), new Vector3(-0.820f, -0.106f, +0.563f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.417f, +0.543f, -0.613f), new Vector3(+0.000f, +0.103f, +0.995f)));

                RCLevelParameters level2Params = new RCLevelParameters(Ctx.ContentRqst, "Tower Level 1",
                    "lava2", "seamless_rock", "lava", 50f, 1, .05f, .075f, .78f, .83f,
                    sceneNode, level2SpawnPoints,"Volcanic Tower Battle",20,10,2);

                levels.Add(new RCLevel(level2Params,levels));
            }
            #endregion level 2

            #region level 3
            {
                List<RCLevelSpawnPoint> level3SpawnPoints = new List<RCLevelSpawnPoint>();
                level3SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.338f, +0.540f, -0.290f), new Vector3(-0.918f, -0.105f, +0.382f)));
                level3SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.912f, +0.535f, -0.302f), new Vector3(-0.045f, -0.029f, +0.084f)));
                level3SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.263f, +0.502f, -0.031f), new Vector3(-0.098f, -0.013f, +0.008f)));
                level3SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.638f, +0.430f, +0.480f), new Vector3(+0.057f, -0.021f, -0.079f)));
                level3SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.209f, +0.441f, +0.917f), new Vector3(+0.003f, -0.005f, -0.100f)));

                RCLevelParameters level3Params = new RCLevelParameters(Ctx.ContentRqst, "Deathmatch Level 2",
                    "tileable_snow", "ice", "snow", 150f, 1, .35f, .65f, .7f, .8f,
                    sceneNode, level3SpawnPoints, "Rough Snowy Plain",30,2,3);

                levels.Add(new RCLevel(level3Params,levels));
            }
            #endregion level 3

            #region level 4
            {
                List<RCLevelSpawnPoint> level4SpawnPoints = new List<RCLevelSpawnPoint>();
                level4SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.338f, +0.540f, -0.290f), new Vector3(-0.918f, -0.105f, +0.382f)));
                level4SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.912f, +0.535f, -0.302f), new Vector3(-0.045f, -0.029f, +0.084f)));
                level4SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.263f, +0.502f, -0.031f), new Vector3(-0.098f, -0.013f, +0.008f)));
                level4SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.638f, +0.430f, +0.480f), new Vector3(+0.057f, -0.021f, -0.079f)));
                level4SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.209f, +0.441f, +0.917f), new Vector3(+0.003f, -0.005f, -0.100f)));

                RCLevelParameters level4Params = new RCLevelParameters(Ctx.ContentRqst, "Trenches",
                    "tileable_snow", "ice", "snow", 150f, 1, .35f, .65f, .7f, .8f,
                    sceneNode, level4SpawnPoints, "Trench War", 30, 2, 3);

                levels.Add(new RCLevel(level4Params, levels));
            }
            #endregion level 4

            #region level 5
            {
                List<RCLevelSpawnPoint> level5SpawnPoints = new List<RCLevelSpawnPoint>();
                level5SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.338f, +0.540f, -0.290f), new Vector3(-0.918f, -0.105f, +0.382f)));
                level5SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.912f, +0.535f, -0.302f), new Vector3(-0.045f, -0.029f, +0.084f)));
                level5SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(+0.263f, +0.502f, -0.031f), new Vector3(-0.098f, -0.013f, +0.008f)));
                level5SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.638f, +0.430f, +0.480f), new Vector3(+0.057f, -0.021f, -0.079f)));
                level5SpawnPoints.Add(new RCLevelSpawnPoint(
                    new Vector3(-0.209f, +0.441f, +0.917f), new Vector3(+0.003f, -0.005f, -0.100f)));

                RCLevelParameters level5Params = new RCLevelParameters(Ctx.ContentRqst, "RandomStuff",
                    "tileable_snow", "ice", "snow", 150f, 1, .35f, .65f, .7f, .8f,
                    sceneNode, level5SpawnPoints, "Sharp Chaos", 1, 1, 1);

                levels.Add(new RCLevel(level5Params, levels));
            }
            #endregion level 5

            levels["Grassy Canyon Battle"].LoadLevel();
            //levels["Volcanic Tower Battle"].LoadLevel();
            //levels["Rough Snowy Plain"].LoadLevel();
            //levels["Trench War"].LoadLevel();
            //levels["Sharp Chaos"].LoadLevel();
            

        }
    }
}
