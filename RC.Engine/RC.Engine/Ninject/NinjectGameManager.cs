using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Core;
using Ninject.Integration.DynamicProxy2;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RC.Engine.StateManagement;
using RC.Engine.Rendering;
using RC.Engine.Cameras;
using RC.Engine.ContentManagement;
using Microsoft.Xna.Framework;
using Ninject.Conditions;
using RC.Engine.Base;
using Ninject.Core.Behavior;
using RC.Engine.Plugin;

namespace RC.Engine.Ninject
{
    internal class NinjectGameManager : RCGameManager
    {
        /// <summary>
        /// I am the sole game kernel and control all the dependencies 
        /// that need to be injected.
        /// </summary>
        private IKernel GameKernel = null;

        public NinjectGameManager()
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
        public T RegisterBaseType<T>(String tag, Type bindType) where T : RCBase
        {
            GameKernel.Load(new InlineModule(delegate(InlineModule m)
            {
                m.Bind<T>().
                    To(bindType).
                    Only(When.Context.Target.Tag.EqualTo(tag)).
                    Using<SingletonBehavior>();
            }));

            // TODO: In the case there are two bindings of the same class
            // and a disambiguity needs to be resolved, this should be 
            // changes to verify that the recieved object is the correct 
            // object.  It seems to be fine for now, but could cause problems
            // in the future.
            return GameKernel.Get(bindType) as T;
        }

        public void LoadModule(IRCModule module)
        {
            RCPluginManager pluginMgr = GameKernel.Get<RCPluginManager>();
            module.Plugin(pluginMgr);
        }

        /// <summary>
        /// I bind default dependencies for a inline module.
        /// </summary>
        /// <param name="m">The inline module.</param>
        private void ModuleBind(InlineModule m)
        {
            m.Bind<Game>().
                To<NinjectGame>().
                Using<SingletonBehavior>();

            m.Bind<RCPluginManager>().
                ToSelf().
                Using<SingletonBehavior>();

            m.Bind<INinjectUpdatePlugin>().
                To<NinjectUpdatePlugin>().
                Using<SingletonBehavior>();

            m.Bind<RCGameManager>().
                ToFactoryMethod<RCGameManager>(RCGameManagerFactory);

            m.Bind<RCGameContext>().
                To<NinjectGameContext>().
                Using<SingletonBehavior>();

            m.Bind<IGraphicsDeviceService>().
                ToFactoryMethod<IGraphicsDeviceService>(IGraphicsDeviceServiceFactory);
            
            m.Bind<ContentManager>().
                ToFactoryMethod<ContentManager>(ContentManagerFactory);
            
            m.Bind<IRCGameStateManager>().
                To<RCGameStateManager>().
                Using<SingletonBehavior>();
            
            m.Bind<IRCGameStateStack>().
                To<RCGameStateManager>().
                Using<SingletonBehavior>();
            
            m.Bind<IRCRenderManager>().
                To<RCRenderManager>().
                Using<SingletonBehavior>();
            
            m.Bind<IRCCameraManager>().
                To<RCCameraManager>().
                Using<SingletonBehavior>();
            
            m.Bind<IRCContentRequester>().
                To<RCContentManager>().
                Using<SingletonBehavior>();
        }

        private RCGameManager RCGameManagerFactory()
        {
            return this;
        }

        /// <summary>
        /// I act as a factory to provide an instance of the graphice device.
        /// </summary>
        /// <returns>The graphics device.</returns>
        private IGraphicsDeviceService IGraphicsDeviceServiceFactory()
        {
            Game game = GameKernel.Get<Game>();
            return game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        }

        /// <summary>
        /// I act as a factory to provide an instance of the content manager.
        /// </summary>
        /// <returns>The content manager.</returns>
        private ContentManager ContentManagerFactory()
        {
            Game game = GameKernel.Get<Game>();
            return game.Content;
        }
    }
}