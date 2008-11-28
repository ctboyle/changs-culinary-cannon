using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RC.Engine.Ninject
{
    public interface INinjectUpdatePlugin
    {
        [NinjectUpdatePlugin]
        void Update(GameTime gameTime);
    }

    public class NinjectUpdatePlugin : INinjectUpdatePlugin
    {
        #region IUpdatePlug Members

        [NinjectUpdatePlugin]
        public virtual void Update(GameTime gameTime)
        {
        }

        #endregion
    }
}