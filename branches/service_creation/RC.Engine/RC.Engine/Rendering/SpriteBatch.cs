using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.ContentManagement;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    public class RCSpriteBatch : RCDeviceResource
    {
        private SpriteBatch _spriteBatch = null;

        public RCSpriteBatch() :
            base()
        {
        }

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        protected override bool IsOnDevice
        {
            get { return (_spriteBatch != null); }
        }

        protected override void SetOnDevice()
        {
            _spriteBatch = new SpriteBatch(Graphics.GraphicsDevice);
        }

        protected override void RemoveFromDevice()
        {
            if (_spriteBatch != null)
            {
                _spriteBatch.Dispose();
                _spriteBatch = null;
            }
        }
    }
}
