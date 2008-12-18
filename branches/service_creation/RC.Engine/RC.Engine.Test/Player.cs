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
using JigLibX.Geometry;
using RC.Engine.Test.Particle;
using JigLibX.Collision;

namespace RC.Engine.Test
{
    public class Player
    {
        static Random random = new Random();

        private const double MaxDeadLength = 5.0;

        private ParticleSystem _fireParticles; 
        private ParticleSystem _smokeParticles; 
        private RCCamera _playerCamera;
        private RCModelContent _potatoGun;
        private PlayerIndex _playerIndex;
        private PotatoPool _pool;
        private RCSceneNode gunPivot;
        private Car car;

        private Vector3 _potatoVel = Vector3.Zero;

        private double _timeSinceLasFire = 0.0;
        private double _deadDuration = 0.0;
        private bool dead = false;

        private int _numPlayers = 0;

        private IRCCameraManager _cameraMgr;
        private IGraphicsDeviceService _graphics;

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

        public Player(IServiceProvider services, PlayerIndex playerIndex, PotatoPool pool, int numPlayers)
        {
            #region Get Required Services
            _cameraMgr = (IRCCameraManager)services.GetService(typeof(IRCCameraManager));
            _graphics = (IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService));
            #endregion

            _playerIndex = playerIndex;
            _pool = pool;
            _numPlayers = numPlayers;
        }

        public void Die()
        {
            dead = true;
            _deadDuration = 0.0;
        }

        public void CreatePlayerCamera(Viewport screen)
        {
            int numPlayersPerRow = 1;
            int numRows = _numPlayers > 1 ? 2 : 1; 

            int numColumns =  _numPlayers > 2 ? 2 : 1; 

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

            _cameraMgr.AddCamera(PlayerCameraLabel, _playerCamera);
        }

        public void CreatePlayerContent(RCSceneNode root)
        {
            RCModelContent carModel;
            JigLibXVehicle carPhysics;

            CreateCar(out carModel, out car, out carPhysics);
            car.Chassis.Body.MoveTo(new Vector3(0.0f, 10.0f, 0.0f), Matrix.Identity);

            CreateGun();
            _potatoGun.Content.LocalTrans = Matrix.CreateTranslation(0.0f, 0.0f, 1.0f);
            
            gunPivot = new RCSceneNode();
            gunPivot.LocalTrans = Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-1.0f, 2.0f, 0.0f)) ;

            CreatePlayerCamera(_graphics.GraphicsDevice.Viewport);
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

        private void CreateGun()
        {
            _potatoGun = new RCModelContent(@"Content\Models\potatoGun");
            RCMaterialState material = new RCMaterialState();
            material.Ambient = new Color(new Vector3(0.4f));

            _fireParticles = new ExplosionSmokeParticleSystem(_graphics, new ParticleEffect());
            _smokeParticles = new SmokePlumeParticleSystem(_graphics, new ParticleEffect());
            _potatoGun.Content.AddChild(_fireParticles);
            _potatoGun.Content.AddChild(_smokeParticles);
        }

        private void CreateCar(out RCModelContent carModel, out Car car, out JigLibXVehicle carPhysics)
        {
            carModel = new RCModelContent(@"Content\Models\Car");

            RCModelContent wheelDrawable1 = new RCModelContent(@"Content\Models\wheel");
            RCModelContent wheelDrawable2 = new RCModelContent(@"Content\Models\wheel");
            RCModelContent wheelDrawable3 = new RCModelContent(@"Content\Models\wheel");
            RCModelContent wheelDrawable4 = new RCModelContent(@"Content\Models\wheel");

            JigLibXWheel wheel1 = new JigLibXWheel(wheelDrawable1);
            JigLibXWheel wheel2 = new JigLibXWheel(wheelDrawable2);
            JigLibXWheel wheel3 = new JigLibXWheel(wheelDrawable3);
            JigLibXWheel wheel4 = new JigLibXWheel(wheelDrawable4);

            car = new Car(true, true, 60.0f, 20.0f, 4.7f, 5.0f, 0.50f, 0.4f, 0.00f,
              0.45f, 0.3f, 1, 300.0f, PhysicsSystem.CurrentPhysicsSystem.Gravity.Length());

            carPhysics = new JigLibXVehicle(car, carModel, wheel1, wheel2, wheel3, wheel4, this); 
        }

        public void UpdateInput(GameTime gameTime, GamePadState padState)
        {

            if (!dead)
            {
                _timeSinceLasFire += gameTime.ElapsedGameTime.TotalSeconds;

                if (padState.Triggers.Right > 0.75f)
                {
                    if (_timeSinceLasFire >= 0.25)
                    {
                        _timeSinceLasFire = 0.0;

                        Vector3 gunPos;
                        Quaternion gunRot;
                        Vector3 scale;

                        _potatoGun.Content.WorldTrans.Decompose(out scale,
                            out gunRot,
                            out gunPos);



                        _pool.FirePotato(gunPos + 5.0f * _potatoGun.Content.WorldTrans.Forward, Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateFromQuaternion(gunRot), _potatoGun.Content.WorldTrans.Forward * 75.0f);

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
            else
            {
                _deadDuration += gameTime.ElapsedGameTime.TotalSeconds;

                _fireParticles.AddParticle(gunPivot.WorldTrans.Translation + .75f * _potatoGun.Content.WorldTrans.Forward , Vector3.Zero);
                _smokeParticles.AddParticle(gunPivot.WorldTrans.Translation + .75f * _potatoGun.Content.WorldTrans.Forward, Vector3.Zero);

                if (_deadDuration > MaxDeadLength)
                {
                    dead = false;
                }

            }
        }

        Vector3 RandomPointOnCircle()
        {
            const float radius = 1;
            const float height = 1;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector3(x * radius, 0, y * radius + height);
        }

    }
}
