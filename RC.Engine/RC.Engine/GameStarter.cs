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
using Ninject.Conditions;
using System.Diagnostics;

namespace RC.Engine
{
    /// <summary>
    /// I the Game Starter.  I start the game.
    /// </summary>
    public static class RCGameStarter
    {
        private static IKernel GameKernel = null;

        public static void Start(Type gameType)
        {
            SetupKernel(new InlineModule(delegate(InlineModule m)
            {
                m.Bind<RCBasicGame>().
                    To(gameType);
            }));

            using (GameKernel.BeginScope())
            {
                RCGame game = GameKernel.Get<RCGame>();
                game.Run();
            }
        }

        public static T BindTagObject<T>(String tag, Type bindType) where T : class
        {
            GameKernel.Load(new InlineModule(delegate(InlineModule m)
            {
                m.Bind<T>().
                    To(bindType).
                    Only(When.Context.Target.Tag.EqualTo(tag));
            }));

            // TODO: In the case there are two bindings of the same class
            // and a disambiguity needs to be resolved, this should be 
            // changes to verify that the recieved object is the correct 
            // object.  It seems to be fine for now, but could cause problems
            // in the future.
            return GameKernel.Get(bindType) as T;
        }

        private static void SetupKernel(params IModule[] modules)
        {
            List<IModule> moduleList = new List<IModule>();

            if (modules != null)
            {
                moduleList.AddRange(modules);
            }

            SetupInternalModules(moduleList);

            GameKernel = new StandardKernel(moduleList.ToArray());
        }

        private static void SetupInternalModules(List<IModule> moduleList)
        {
            moduleList.Add(new InlineModule(ModuleBind));
        }

        private static void ModuleBind(InlineModule m)
        {
            m.Bind<IGraphicsDeviceService>().ToFactoryMethod<IGraphicsDeviceService>(IGraphicsDeviceServiceFactory);
            m.Bind<ContentManager>().ToFactoryMethod<ContentManager>(ContentManagerFactory);
            m.Bind<IRCGameStateManager>().To<RCGameStateManager>();
            m.Bind<IRCGameStateStack>().To<RCGameStateManager>();
            m.Bind<IRCRenderManager>().To<RCRenderManager>();
            m.Bind<IRCCameraManager>().To<RCCameraManager>();
            m.Bind<IRCContentRequester>().To<RCContentManager>();
        }

        private static IGraphicsDeviceService IGraphicsDeviceServiceFactory()
        {
            RCGame game = GameKernel.Get<RCGame>();
            return game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        }

        private static ContentManager ContentManagerFactory()
        {
            RCGame game = GameKernel.Get<RCGame>();
            return game.Content;
        }
    }
}