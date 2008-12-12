using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Cameras;
using RC.Engine.ContentManagement.ContentTypes;
using Microsoft.Xna.Framework;
using RC.Engine.ContentManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Base;
using RC.Engine.GraphicsManagement;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Input;
using RC.Physics;
using JigLibX.Vehicles;
using JigLibX.Physics;
using RC.Engine.Test.Particle;

namespace RC.Engine.Test
{
    class Player
    {
        static Random random = new Random();

        private ParticleSystem _fireParticles; 
        private ParticleSystem _smokeParticles; 
        private RCCamera _playerCamera;
        private RCModelContent _potatoGun;
        private PlayerIndex _playerIndex;
        private PotatoPool _pool;
        private RCSceneNode gunPivot;
        private Car car;

        private Vector3 _potatoVel = Vector3.Zero;

        private double _timeSinceLasFire = 0.0f;
        


        private int _numPlayers = 0;

        public RCSceneNode Gun
        {
            get { return _potatoGun.Content; }
        }


        public PlayerIndex PlayerIndex
        {
            get { return _playerIndex; }
            set { _playerIndex = value; }
        }

        public string PlayerCameraLabel
        {
            get { return "Player Camera " + _playerIndex.ToString();}
        }

        public Player(PlayerIndex playerIndex, PotatoPool pool, int numPlayers)
        {
            _playerIndex = playerIndex;
            _pool = pool;
            _numPlayers = numPlayers;

        }

        public void Die()
        {

        }

        public void CreatePlayerCamera(Viewport screen, IRCCameraManager camManager)
        {
            int numPlayersPerRow = 2;
            int numRows = (int)Math.Ceiling((float)(_numPlayers) / numPlayersPerRow);

            int numColumns = _numPlayers > 1 ? 2 : 1; 

            Viewport playerScreenPart = new Viewport();

            playerScreenPart.Width = screen.Width / numColumns;
            playerScreenPart.Height = screen.Height / numRows;

            int playerRowIndex = ((int)(_playerIndex)) / numColumns;
            int playerColIndex = ((int)(_playerIndex)) % numColumns;


            playerScreenPart.X = (int)(playerColIndex) * playerScreenPart.Width;
            playerScreenPart.Y = (int)(playerRowIndex) * playerScreenPart.Height;

            RCPerspectiveCamera playerCamera = new RCPerspectiveCamera(playerScreenPart);
            playerCamera.FOV = MathHelper.ToRadians(60);
            playerCamera.Near = 0.05f;
            _playerCamera = playerCamera;


            

            FlyCameraController controller = new FlyCameraController(5.0f, MathHelper.PiOver2);
            controller.AttachToObject(_playerCamera);

            camManager.AddCamera(PlayerCameraLabel, _playerCamera);
           
        }


        public void CreatePlayerContent(RCSceneNode root, RCGameContext ctx)
        {
            
            RCModelContent carModel;
            JigLibXVehicle carPhysics;

            CreateCar(ctx, out carModel, out car, out carPhysics);
            car.Chassis.Body.MoveTo(new Vector3(0.0f, 10.0f, 0.0f), Matrix.Identity);

            CreateGun(ctx);
            _potatoGun.Content.LocalTrans = Matrix.CreateTranslation(0.0f, 0.0f, 1.0f);
            
            gunPivot = new RCSceneNode();
            gunPivot.LocalTrans = Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-1.0f, 2.0f, 0.0f)) ;

            CreatePlayerCamera(ctx.Graphics.GraphicsDevice.Viewport, ctx.CameraMgr);
            _playerCamera.LocalTrans = Matrix.CreateTranslation(new Vector3(-0.0f, 0.5f, -0.75f));
            
            carModel.Content.AddChild(gunPivot);
            gunPivot.AddChild(_potatoGun);
            _potatoGun.Content.AddChild(_playerCamera);


            root.AddChild(carPhysics);

        }

        public void SetPlayerPosition(RCLevelSpawnPoint point)
        {
            car.Chassis.Body.MoveTo(point.Position, Matrix.CreateWorld(Vector3.Zero, point.Heading, Vector3.Up));
        }

        private void CreateGun(RCGameContext ctx)
        {

            _potatoGun = new RCModelContent(ctx.ContentRqst, @"Content\Models\potatoGun");
            RCMaterialState material = new RCMaterialState();
            material.Ambient = new Color(new Vector3(0.4f));

            _fireParticles = new ExplosionSmokeParticleSystem(ctx, new ParticleEffect(ctx.ContentRqst));
            _smokeParticles = new SmokePlumeParticleSystem(ctx, new ParticleEffect(ctx.ContentRqst));
            _potatoGun.Content.AddChild(_fireParticles);
            _potatoGun.Content.AddChild(_smokeParticles);
        }

        private static void CreateCar(RCGameContext ctx, out RCModelContent carModel, out Car car, out JigLibXVehicle carPhysics)
        {
            carModel = new RCModelContent(ctx.ContentRqst, @"Content\Models\Car");


            RCModelContent wheelDrawable1 = new RCModelContent(ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable2 = new RCModelContent(ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable3 = new RCModelContent(ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable4 = new RCModelContent(ctx.ContentRqst, @"Content\Models\wheel");

            JigLibXWheel wheel1 = new JigLibXWheel(wheelDrawable1);
            JigLibXWheel wheel2 = new JigLibXWheel(wheelDrawable2);
            JigLibXWheel wheel3 = new JigLibXWheel(wheelDrawable3);
            JigLibXWheel wheel4 = new JigLibXWheel(wheelDrawable4);


            car = new Car(true, true, 60.0f, 20.0f, 4.7f, 5.0f, 0.50f, 0.4f, 0.05f,
              0.45f, 0.3f, 1, 300.0f, PhysicsSystem.CurrentPhysicsSystem.Gravity.Length());

            carPhysics = new JigLibXVehicle(car, carModel, wheel1, wheel2, wheel3, wheel4);

            
        }

        public void UpdateInput(GameTime gameTime, GamePadState padState)
        {
            _timeSinceLasFire += gameTime.ElapsedGameTime.TotalSeconds;

            if (padState.Triggers.Right > 0.75f)
            {
                if (_timeSinceLasFire >= 0.5)
                {
                    _timeSinceLasFire = 0.0;

                    Vector3 gunPos;
                    Quaternion gunRot;
                    Vector3 scale;

                    _potatoGun.Content.WorldTrans.Decompose(out scale,
                        out gunRot,
                        out gunPos);



                    _pool.FirePotato(gunPos + 5.0f * _potatoGun.Content.WorldTrans.Forward , Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateFromQuaternion(gunRot), _potatoGun.Content.WorldTrans.Forward * 25.0f);
                    
                    // Add effect of gun
                    _fireParticles.AddParticle(gunPos + 5f * _potatoGun.Content.WorldTrans.Forward, Vector3.Zero);
                    _smokeParticles.AddParticle(gunPos + 5f * _potatoGun.Content.WorldTrans.Forward, Vector3.Zero);
                }
            }

            _potatoGun.Content.LocalTrans *= 
                Matrix.CreateFromAxisAngle(Vector3.Up, -(float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver2 * padState.ThumbSticks.Right.X) *
                Matrix.CreateFromAxisAngle(-_potatoGun.Content.LocalTrans.Right, (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver2 * padState.ThumbSticks.Right.Y);


            
            car.Accelerate = padState.ThumbSticks.Left.Y;

            car.Steer = -padState.ThumbSticks.Left.X;

            car.HBrake = padState.IsButtonDown(Buttons.LeftShoulder) ? 1 : 0;

            if (padState.IsButtonDown(Buttons.Back))
            {
                SetPlayerPosition(new RCLevelSpawnPoint(car.Chassis.Body.Position, Vector3.Forward));
            }


        }

        Vector3 RandomPointOnCircle()
        {
            const float radius = 30;
            const float height = 40;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector3(x * radius, y * radius + height, 0);
        }

    }
}
