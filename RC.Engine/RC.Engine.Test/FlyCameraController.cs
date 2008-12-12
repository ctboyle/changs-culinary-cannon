using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Animation;
using RC.Engine.Cameras;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace RC.Engine.Test
{
    class FlyCameraController : Controller<RCCamera>
    {
        private float _rotateSpeed = 1.0f;
        private float _travelSpeed = 1.0f;

        private Vector2 _angles;
        private Vector3 _cameraPos;

        public FlyCameraController( float travelUnitPerSec, float rotateSpeedRadPerSec)
        {
            _rotateSpeed = rotateSpeedRadPerSec;
            _travelSpeed = travelUnitPerSec;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            Vector2 rotIncrement = Vector2.Zero;
            Vector3 transIncrement = Vector3.Zero;

            if (newState.IsKeyDown(Keys.Up))
            {
                transIncrement += _controlledItem.LocalTrans.Forward;
            }

            if (newState.IsKeyDown(Keys.Down))
            {
                transIncrement -= _controlledItem.LocalTrans.Forward;
            }

            if (newState.IsKeyDown(Keys.Left))
            {
                transIncrement -= _controlledItem.LocalTrans.Right;
            }

            if (newState.IsKeyDown(Keys.Right))
            {
                transIncrement += _controlledItem.LocalTrans.Right;
            }

            if (newState.IsKeyDown(Keys.A))
            {
                rotIncrement.Y += 1.0f;
            }

            if (newState.IsKeyDown(Keys.D))
            {
                rotIncrement.Y -= 1.0f;
            }

            if (newState.IsKeyDown(Keys.W))
            {
                rotIncrement.X -= 1.0f;
            }

            if (newState.IsKeyDown(Keys.S))
            {
                rotIncrement.X += 1.0f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                System.Diagnostics.Debugger.Break();
            }


            transIncrement *= ((float)gameTime.ElapsedGameTime.TotalSeconds) * _travelSpeed;
            rotIncrement *= ((float)gameTime.ElapsedGameTime.TotalSeconds) * _rotateSpeed;

            _cameraPos += transIncrement;
            _angles += rotIncrement;

            _controlledItem.LocalTrans = 
                Matrix.CreateFromYawPitchRoll(_angles.Y * _rotateSpeed, _angles.X, 0) * Matrix.CreateTranslation(_cameraPos);

        }
    }
}
