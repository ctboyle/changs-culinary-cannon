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

namespace RC.Engine.Test
{
    class Player
    {
        private RCCamera _playerCamera;
        private RCModelContent _potatoGun;
        private PlayerIndex _playerIndex;


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

        public Player(PlayerIndex playerIndex, int numPlayers)
        {
            _playerIndex = playerIndex;

            _numPlayers = numPlayers;

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

            camManager.AddCamera(PlayerCameraLabel, _playerCamera);
           
        }


        public RCSpatial CreatePlayerContent(RCGameContext ctx)
        {
            _potatoGun = new RCModelContent(ctx.ContentRqst, @"Content\Models\potatoGun");
            RCMaterialState material = new RCMaterialState();
            material.Ambient = new Color(new Vector3(0.4f));



            CreatePlayerCamera(ctx.Graphics.GraphicsDevice.Viewport, ctx.CameraMgr);

            _potatoGun.Content.AddChild(_playerCamera);
            _playerCamera.LocalTrans = Matrix.CreateTranslation(new Vector3(-0.05f, 0.05f, -0.1f));

            return _potatoGun;

        }

    }
}
