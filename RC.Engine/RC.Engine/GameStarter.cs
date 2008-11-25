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
    /// I am the Game Starter.  I start the game and bind new dependencies
    /// to the game kernel.  
    /// </summary>
    public static class RCGameStarter
    {
        /// <summary>
        /// I am the sole game kernel and control all the dependencies 
        /// that need to be injected.
        /// </summary>
        private static IKernel GameKernel = null;

        /// <summary>
        /// I start a new game, given a <paramref name="gameType"/> that is 
        /// an instance of <typeparamref name="RCBasicGame"/>.
        /// </summary>
        /// <param name="gameType">The game type.</param>
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

        /// <summary>
        /// Binds new types to be injected at runtime.  To reduce the 
        /// possibility of ambiguity, a tag will be required for the injection.
        /// </summary>
        /// <typeparam name="T">The injection type.</typeparam>
        /// <param name="tag">A string tag to represent.</param>
        /// <param name="bindType">The actual type.</param>
        /// <returns>An instance of the binded object.</returns>
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

        /// <summary>
        /// I setup a new kernel for a new game's set of dependencies.  The
        /// modules setup in the kernel are provided here, plus the internal
        /// modules that are to be used for all games.
        /// </summary>
        /// <param name="modules">The modules.</param>
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

        /// <summary>
        /// I setup the depedency modules for all games.
        /// </summary>
        /// <param name="moduleList"></param>
        private static void SetupInternalModules(List<IModule> moduleList)
        {
            moduleList.Add(new InlineModule(ModuleBind));
        }

        /// <summary>
        /// I bind default dependencies for a inline module.
        /// </summary>
        /// <param name="m">The inline module.</param>
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

        /// <summary>
        /// I act as a factory to provide an instance of the graphice device.
        /// </summary>
        /// <returns>The graphics device.</returns>
        private static IGraphicsDeviceService IGraphicsDeviceServiceFactory()
        {
            RCGame game = GameKernel.Get<RCGame>();
            return game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        }

        /// <summary>
        /// I act as a factory to provide an instance of the content manager.
        /// </summary>
        /// <returns>The content manager.</returns>
        private static ContentManager ContentManagerFactory()
        {
            RCGame game = GameKernel.Get<RCGame>();
            return game.Content;
        }
    }
}