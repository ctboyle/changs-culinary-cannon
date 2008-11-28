using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Ninject;
using RC.Engine.Base;

namespace RC.Engine
{
    public class RCDefaultGameManagerFactory : RCGameManagerFactory
    {
        #region GameManagerFactory Members

        public RCGameManager GetInstance()
        {
            return new NinjectGameManager();
        }

        #endregion
    }
}