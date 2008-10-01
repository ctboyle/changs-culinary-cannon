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

        public FlyCameraController( float travelUnitPerSec, float rotateSpeedRadPerSec)
        {
            _rotateSpeed = rotateSpeedRadPerSec;
            _travelSpeed = travelUnitPerSec;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;
            float yAxisRot = 0.0f;
            float xAxisRot = 0.0f;

            if (newState.IsKeyDown(Keys.Up))
            {
                moveVector += _controlledItem.LocalTrans.Forward;
            }

            if (newState.IsKeyDown(Keys.Down))
            {
                moveVector -= _controlledItem.LocalTrans.Forward;
            }

            if (newState.IsKeyDown(Keys.Left))
            {
                moveVector -= _controlledItem.LocalTrans.Right;
            }

            if (newState.IsKeyDown(Keys.Right))
            {
                moveVector += _controlledItem.LocalTrans.Right;
            }

            if (newState.IsKeyDown(Keys.A))
            {
                yAxisRot += 1.0f;
            }

            if (newState.IsKeyDown(Keys.D))
            {
                yAxisRot -= 1.0f;
            }

            if (newState.IsKeyDown(Keys.W))
            {
                xAxisRot -= 1.0f;
            }

            if (newState.IsKeyDown(Keys.S))
            {
                xAxisRot += 1.0f;
            }

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 saveTranslation = _controlledItem.LocalTrans.Translation;

            _controlledItem.LocalTrans *= Matrix.CreateTranslation(-_controlledItem.LocalTrans.Translation);
                
            _controlledItem.LocalTrans *=  Matrix.CreateFromAxisAngle(_controlledItem.LocalTrans.Right, xAxisRot * _rotateSpeed * time) *
                Matrix.CreateFromAxisAngle(Vector3.Up, yAxisRot * _rotateSpeed * time);

            _controlledItem.LocalTrans *= Matrix.CreateTranslation(saveTranslation+moveVector * _travelSpeed * time);
        }
    }
}
