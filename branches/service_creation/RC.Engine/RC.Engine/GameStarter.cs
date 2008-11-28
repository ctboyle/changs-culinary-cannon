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
using Ninject.Integration.DynamicProxy2;

namespace RC.Engine
{
    /// <summary>
    /// I am the Game Starter.  I start the game and bind new dependencies
    /// to the game kernel.  
    /// </summary>
    public class RCGameStarter
    {
        /// <summary>
        /// I am the sole game kernel and control all the dependencies 
        /// that need to be injected.
        /// </summary>
        private static IKernel GameKernel = null;

        public RCGameStarter()
        {
            GameKernel = new StandardKernel(
                new DynamicProxy2Module(), 
                new InlineModule(ModuleBind)
            );
        }

        /// <summary>
        /// I start a new game, given a <paramref name="gameType"/> that is 
        /// an instance of <typeparamref name="RCBasicGame"/>.
        /// </summary>
        /// <param name="gameType">The game type.</param>
        public void Start(Type gameType)
        {
            GameKernel.Load((new InlineModule(delegate(InlineModule m)
            {
                m.Bind<RCBasicGame>().
                    To(gameType);
            })));

            using (GameKernel.BeginScope())
            {
                Game game = GameKernel.Get<Game>();
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
        public T BindTagObject<T>(String tag, Type bindType) where T : class
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

        public void LoadNewModule(RCModule module)
        {
            ModuleImpl ninjectModule = new ModuleImpl(module);
            GameKernel.Load(ninjectModule);
        }

        /// <summary>
        /// I bind default dependencies for a inline module.
        /// </summary>
        /// <param name="m">The inline module.</param>
        private void ModuleBind(InlineModule m)
        {
            m.Bind<Game>().To<RCGame>();
            m.Bind<IGraphicsDeviceService>().ToFactoryMethod<IGraphicsDeviceService>(IGraphicsDeviceServiceFactory);
            m.Bind<ContentManager>().ToFactoryMethod<ContentManager>(ContentManagerFactory);
            m.Bind<IRCGameStateManager>().To<RCGameStateManager>();
            m.Bind<IRCGameStateStack>().To<RCGameStateManager>();
            m.Bind<IRCRenderManager>().To<RCRenderManager>();
            m.Bind<IRCCameraManager>().To<RCCameraManager>();
            m.Bind<IRCContentRequester>().To<RCContentManager>();
            m.Bind<RCGameStarter>().ToFactoryMethod<RCGameStarter>(RCGameStarterFactory);
        }

        private RCGameStarter RCGameStarterFactory()
        {
            return this;
        }

        /// <summary>
        /// I act as a factory to provide an instance of the graphice device.
        /// </summary>
        /// <returns>The graphics device.</returns>
        private IGraphicsDeviceService IGraphicsDeviceServiceFactory()
        {
            RCGame game = GameKernel.Get<RCGame>();
            return game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        }

        /// <summary>
        /// I act as a factory to provide an instance of the content manager.
        /// </summary>
        /// <returns>The content manager.</returns>
        private ContentManager ContentManagerFactory()
        {
            RCGame game = GameKernel.Get<RCGame>();
            return game.Content;
        }
    }
}