using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.ContentManagement
{
    public abstract class RCDeviceResource : IDisposable
    {
        private IGraphicsDeviceService _graphics = null;

        public RCDeviceResource()
        {

        }

        ~RCDeviceResource()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            //RemoveFromDevice();
            GC.SuppressFinalize(this);
        }

        #endregion


        public void Enable(IGraphicsDeviceService graphics)
        {
            if (!IsOnDevice)
            {
                _graphics = graphics;
                SetOnDevice();
            }
        }

        public void Disable()
        {
            if (IsOnDevice)
            {
                RemoveFromDevice();
                _graphics = null;
            }
        }


        protected abstract bool IsOnDevice { get; }

        protected abstract void SetOnDevice();

        protected abstract void RemoveFromDevice();

        protected IGraphicsDeviceService Graphics
        {
            get { return _graphics; }
        }
    }
}