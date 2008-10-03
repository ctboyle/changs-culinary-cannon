using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.ContentManagement
{
    public abstract class RCDeviceResource : IDisposable
    {
        private IGraphicsDeviceService _graphics = null;

        public RCDeviceResource(IGraphicsDeviceService graphics)
        {
            _graphics = graphics;
        }

        ~RCDeviceResource()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveFromDevice();
            GC.SuppressFinalize(this);
        }

        #endregion

        public bool Enabled
        {
            get { return IsOnDevice; }
            set 
            {
                if(IsOnDevice && !value)
                    RemoveFromDevice();
                else if (!IsOnDevice && value)
                    SetOnDevice();
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