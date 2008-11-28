using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Core;
using Microsoft.Xna.Framework;
using Ninject.Core.Interception;
using RC.Engine.Plugin;

namespace RC.Engine.Ninject
{
    public class NinjectUpdatePluginInterceptor : SimpleInterceptor
    {
        private RCPluginManager _pluginMgr = null;

        protected override void BeforeInvoke(IInvocation invocation)
        {
            GameTime gameTime = (GameTime)invocation.Request.Arguments[0];

            PluginMgr.RaiseUpdateEvent(new RCPluginManager.GameTimeEventArgs(gameTime));

            base.BeforeInvoke(invocation);
        }

        [Inject]
        public RCPluginManager PluginMgr
        {
            get { return _pluginMgr; }
            set { _pluginMgr = value; }
        }
    }
}
