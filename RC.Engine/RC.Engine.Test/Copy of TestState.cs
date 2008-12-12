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
using JigLibX.Vehicles;
using JigLibX.Physics;

namespace RC.Engine.Test
{
    class TestState : RCGameState
    {
        private RCSpatial _sceneRoot = null;
        private TimeSpan _lastFrameReportTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private int _framesPerSecond = 0;
        private RCSpriteBatch _spriteBatch = null;
        private RCContent<SpriteFont> _spriteFont = null;
        //private float heightMapScaling = 10.0f; //SET THIS PER LEVEL!

        public TestState(RCGameContext gameCtx)
            : base(gameCtx)
        {
        }

        public override void Initialize()
        {
            RCSceneNode sceneRoot = new RCSceneNode();

            /////////////////////////////////////////////////////////////////////
            // Setup the graphics device
            /////////////////////////////////////////////////////////////////////
            Ctx.Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            /////////////////////////////////////////////////////////////////////
            // Setup the SpriteBatch content for displaying the FPS text
            /////////////////////////////////////////////////////////////////////
            _spriteBatch = new RCSpriteBatch();

            _spriteBatch.Enable(Ctx.Graphics);
            _spriteFont = new RCDefaultContent<SpriteFont>(Ctx.ContentRqst, "Content\\Fonts\\DefaultFont");

            /////////////////////////////////////////////////////////////////////
            // Create and setup the camera
            /////////////////////////////////////////////////////////////////////
            RCCamera camera = new RCPerspectiveCamera(Ctx.Graphics.GraphicsDevice.Viewport);
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 40.0f, 10.0f), -Vector3.UnitZ, Vector3.Up);
            Vector3 truckStartPosition = new Vector3(-10.4f, 12.8f, -25.15f);
            Matrix truckStartCam = Matrix.CreateLookAt(truckStartPosition,
                truckStartPosition + new Vector3(-.55f, -0.4f, .5f), Vector3.Up);

            Matrix upLookingDown = Matrix.CreateLookAt(new Vector3(0f, 40f, 0f), -Vector3.UnitZ, Vector3.Up);
            //camera.LocalTrans = Matrix.Invert(cameraLookAt);
            camera.LocalTrans = Matrix.Invert(upLookingDown);
            //camera.LocalTrans = Matrix.Invert(truckStartCam);
            camera.Near = 0.05f;
            Ctx.CameraMgr.AddCamera("Test", camera);

            /////////////////////////////////////////////////////////////////////
            // Add a controller for the camera
            /////////////////////////////////////////////////////////////////////
            FlyCameraController cameraController = new FlyCameraController(5f, MathHelper.PiOver2);
            //cameraController.AttachToObject(camera);
            

            /////////////////////////////////////////////////////////////////////
            // Create and setup the light
            /////////////////////////////////////////////////////////////////////
            //RCSceneNode sceneNode = new RCSceneNode();
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0f, 25.0f, 0f), Vector3.UnitZ, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);
            //sceneNode.SetLight(light);
            //sceneNode.AddLight(light);

            /////////////////////////////////////////////////////////////////////
            // Create the heightmap
            /////////////////////////////////////////////////////////////////////


            //RCContent<RCHeightMap> heightMap = new RCDefaultContent<RCHeightMap>(Ctx.ContentRqst, "Content\\Textures\\Deathmatch Level 1");
            //RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tilable_long_grass");
            //RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\seamless_rock");
            //RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tileable_snow");
            //heightMap.Content.Scaling = heightMapScaling;

            //HeightMapEffect effect = new HeightMapEffect(Ctx.ContentRqst, heightMap,
            //    texture1, texture2, texture3, .3f, .55f, .65f, .75f);
            //heightMap.Content.AddEffect(effect);
            //JibLibXPhysicsObject physicsHeightMap = JibLibXPhysicsHelper.CreateHeightmap(heightMap);

            RCLevelCollection levels = new RCLevelCollection();
            SetUpLevels(sceneRoot, levels);

            /////////////////////////////////////////////////////////////////////
            // Create the models
            /////////////////////////////////////////////////////////////////////
            JigLibXVehicle car = CreateVehicle();

            car.VehicleData.Chassis.Body.MoveTo(levels["Tower 1"].SpawnPoints[0].GetScaledPosition(25f,1f),Matrix.Identity);
            
            //for (int i = 0; i < 25; ++i)
            //{
            //    RCModelContent enemy = new RCModelContent(Ctx.ContentRqst, @"Content\Models\treasure_chest");
            //    enemy.Content.WorldTrans = Matrix.CreateTranslation(new Vector3(0, 20f + (float)2 * i, 3f));
            //    //physicsEnemy = JibLibXPhysicsHelper.CreateObject(enemy);
            //    //physicsEnemy.SetMass(10.0f);
            //    JigLibX.Geometry.Box box = new JigLibX.Geometry.Box(-0.5f * new Vector3(0.5f), Matrix.Identity, new Vector3(0.5f));
            //    //physicsEnemy.Body.CollisionSkin.AddPrimitive(
            //        //box,
            //        //(int)JigLibX.Collision.MaterialTable.MaterialID.UserDefined,
            //        //new JigLibX.Collision.MaterialProperties(0.2f, 0.9f, 0.9f)
            //        //);

            //    RCDepthBufferState depthState = new RCDepthBufferState();
            //    depthState.DepthTestingEnabled = true;
            //    enemy.Content.GlobalStates.Add(depthState);

            //    //sceneNode.AddChild(physicsEnemy);
            //}

            //RCModelContent car = new RCModelContent(Ctx.ContentRqst, @"Content\Models\car");

            /////////////////////////////////////////////////////////////////////
            // Setup the light node as the root and setup its children
            /////////////////////////////////////////////////////////////////////
            //sceneNode.AddChild(camera);
            sceneRoot.AddLight(light);
            sceneRoot.AddChild(camera);
            //sceneRoot.AddChild(heightmap);
            sceneRoot.AddChild(car);

            //_sceneRoot = sceneNode;
            RCDepthBufferState depthState1 = new RCDepthBufferState();
            depthState1.DepthTestingEnabled = true;
            sceneRoot.GlobalStates.Add(depthState1);

            _sceneRoot = sceneRoot;
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

            Ctx.CameraMgr.SetActiveCamera("Test");
            Ctx.RenderMgr.Draw(_sceneRoot);

            // Draw the scene statistics
            string message = string.Format("FPS: {0}\nPosition: {1}\nFacing: {2} \n", _framesPerSecond, Ctx.CameraMgr.ActiveCamera.WorldTrans.Translation, Ctx.CameraMgr.ActiveCamera.WorldTrans.Forward);

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

        //private JibLibXObject CreateHeightmap(float heightMapScaling)
        //{
        //    RCContent<RCHeightMap> heightMap = new RCDefaultContent<RCHeightMap>(Ctx.ContentRqst, "Content\\Textures\\final_heightmap");
        //    RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tilable_long_grass");
        //    RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\seamless_rock");
        //    RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tileable_snow");
        //    heightMap.Content.Scaling = heightMapScaling;

        //    HeightMapEffect effect = new HeightMapEffect(Ctx.ContentRqst, heightMap,
        //        texture1, texture2, texture3, .3f, .55f, .65f, .75f);
        //    heightMap.Content.AddEffect(effect);

        //    JibLibXObject physicsHeightMap = JibLibXPhysicsHelper.CreateHeightmap(heightMap);

        //    return physicsHeightMap;
        //}

        private JigLibXVehicle CreateVehicle()
        {
            RCModelContent wheelDrawable1 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable2 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable3 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable4 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent carDrawable = new RCModelContent(Ctx.ContentRqst, @"Content\Models\car");
            carDrawable.Content.WorldTrans = Matrix.CreateTranslation(new Vector3(0, 50f, 0));

            JigLibXWheel wheel1 = new JigLibXWheel(wheelDrawable1);
            JigLibXWheel wheel2 = new JigLibXWheel(wheelDrawable2);
            JigLibXWheel wheel3 = new JigLibXWheel(wheelDrawable3);
            JigLibXWheel wheel4 = new JigLibXWheel(wheelDrawable4);

            Car car = new Car(true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f, 0.05f,
                0.45f, 0.3f, 1, 520.0f, PhysicsSystem.CurrentPhysicsSystem.Gravity.Length());

            JigLibXVehicle carPhysics = new JigLibXVehicle(car, carDrawable, wheel1, wheel2, wheel3, wheel4);

            return carPhysics;
        }

        private void UpdateInput()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
            {
                Ctx.StateStack.PopState();
            }
        }

        private void SetUpLevels(RCSceneNode lightNode, RCLevelCollection levels)
        {
            #region level 1
            {
                List<RCLevelSpawnPoints> level1SpawnPoints = new List<RCLevelSpawnPoints>();
                level1SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(+0.779f, +0.406f, -0.728f), new Vector3(-0.003f, -0.013f, +0.099f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(+0.912f, +0.535f, -0.302f), new Vector3(-0.045f, -0.029f, +0.084f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(+0.263f, +0.502f, -0.031f), new Vector3(-0.098f, -0.013f, +0.008f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.638f, +0.430f, +0.480f), new Vector3(+0.057f, -0.021f, -0.079f)));
                level1SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.209f, +0.441f, +0.917f), new Vector3(+0.003f, -0.005f, -0.100f)));

                RCLevelParameters level1Params = new RCLevelParameters(Ctx.ContentRqst, "Deathmatch Level 1",
                    "tilable_long_grass", "seamless_rock", "tileable_snow", 10f, 1, .3f, .55f, .65f, .75f,
                    lightNode, level1SpawnPoints);

                levels.Add("Deathmatch 1", new RCLevel(level1Params));
            }
            #endregion level 1

            #region level 2
            {
                List<RCLevelSpawnPoints> level2SpawnPoints = new List<RCLevelSpawnPoints>();
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.554f, +0.523f, -0.646f), new Vector3(+0.688f, +0.025f, +0.725f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.554f, +0.512f, -0.023f), new Vector3(+0.999f, +0.025f, -0.026f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.079f, +0.525f, +0.571f), new Vector3(+0.078f, +0.025f, -0.997f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.585f, +0.537f, +0.581f), new Vector3(+0.743f, +0.025f, -0.669f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(+0.614f, +0.551f, +0.555f), new Vector3(-0.758f, -0.080f, -0.647f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(+0.702f, +0.564f, -0.698f), new Vector3(-0.994f, -0.028f, -0.104f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(+0.650f, +0.556f, -0.626f), new Vector3(-0.820f, -0.106f, +0.563f)));
                level2SpawnPoints.Add(new RCLevelSpawnPoints(
                    new Vector3(-0.417f, +0.543f, -0.613f), new Vector3(+0.000f, +0.103f, +0.995f)));

                RCLevelParameters level2Params = new RCLevelParameters(Ctx.ContentRqst, "Tower Level 1",
                    "lava2", "seamless_rock", "lava", 25f, 1, .05f, .075f, .78f, .83f,
                    lightNode, level2SpawnPoints);

                levels.Add("Tower 1", new RCLevel(level2Params));
            }
            #endregion level 2

            levels["Tower 1"].LoadLevel();
        }

    }
}
