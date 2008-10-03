using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Core;
using Microsoft.Xna.Framework;
using System.Reflection;
using Ninject.Core.Activation;
using Ninject.Core.Binding;
using Ninject.Core.Parameters;
using RC.Engine.StateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.Rendering;
using RC.Engine.Cameras;
using RC.Engine.ContentManagement;

namespace RC.Engine
{
    public class RCGameStarter
    {
        private IKernel _kernel = null;

        public RCGameStarter(params IModule[] modules)
        {
            SetupKernel(modules);
        }

        public void Start()
        {
            using (_kernel.BeginScope())
            {
                _kernel.Get<RCBasicGame>().Run();
            }
        }

        private void SetupKernel(IModule[] modules)
        {
            List<IModule> moduleList = new List<IModule>();

            if (modules != null)
            {
                moduleList.AddRange(modules);
            }

            SetupInternalModules(moduleList);

            _kernel = new StandardKernel(moduleList.ToArray());
        }

        private void SetupInternalModules(List<IModule> moduleList)
        {
            moduleList.Add(new InlineModule(ModuleBind));
        }

        private void ModuleBind(InlineModule m)
        {
            m.Bind<IGraphicsDeviceService>().ToFactoryMethod<IGraphicsDeviceService>(IGraphicsDeviceServiceFactory);
            m.Bind<ContentManager>().ToFactoryMethod<ContentManager>(ContentManagerFactory);
            m.Bind<IRCGameStateManager>().To<RCGameStateManager>();
            m.Bind<IRCRenderManager>().To<RCRenderManager>();
            m.Bind<IRCCameraManager>().To<RCCameraManager>();
            m.Bind<IRCContentRequester>().To<RCContentManager>();
        }

        private IGraphicsDeviceService IGraphicsDeviceServiceFactory()
        {
            Game game = _kernel.Get<RCBasicGame>();
            return game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        }

        private ContentManager ContentManagerFactory()
        {
            Game game = _kernel.Get<RCBasicGame>();
            return game.Content;
        }
    }
}