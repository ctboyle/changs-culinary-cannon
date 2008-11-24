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
    public class RCGameStarter
    {
        private IKernel _kernel = null;

        /// <summary>
        /// I construct the game starter.  I get pissy if you give me crap.
        /// </summary>
        /// <param name="game">I am the game type.  Not the game, the type.</param>
        /// <param name="states">I am all of the game state types with labels.</param>
        public RCGameStarter(Type game, KeyValuePair<String, Type>[] states)
        {
            IModule module = new InlineModule(
                delegate(InlineModule m)
                {
                    m.Bind<RCBasicGame>().To(game);

                    Array.ForEach<KeyValuePair<String, Type>>(
                        states,
                        delegate(KeyValuePair<String, Type> state)
                        {
                            m.Bind<RCGameState>().
                                To(state.Value).
                                Only(When.Context.Target.Tag.EqualTo(state.Key));
                        }
                    );
                }
            );

            SetupKernel(new IModule[] { module });
        }

        public void Start()
        {
            using (_kernel.BeginScope())
            {
                RCGame game = _kernel.Get<RCGame>();
                game.Run();
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
            RCGame game = _kernel.Get<RCGame>();
            return game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        }

        private ContentManager ContentManagerFactory()
        {
            RCGame game = _kernel.Get<RCGame>();
            return game.Content;
        }
    }
}