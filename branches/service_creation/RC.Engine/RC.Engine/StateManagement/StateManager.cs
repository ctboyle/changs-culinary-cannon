using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using Ninject.Core;
using RC.Engine.ContentManagement;
using Ninject.Core.Parameters;

namespace RC.Engine.StateManagement
{
    public delegate void StateChangeHandler(RCGameState newState, RCGameState oldState);

    public interface IRCGameStateStack
    {
        void PushState(string label);
        RCGameState PopState();
        RCGameState PeekState();
    }

    public interface IRCGameStateManager : IRCGameStateStack, IGameComponent
    {
        void AddState(string label, Type stateType);
        void RemoveState(string label);
    }

    [Singleton]
    internal class RCGameStateManager : DrawableGameComponent, IRCGameStateManager
    {
        public delegate void StateChangeFunc(RCGameState previousState, RCGameState newState);

        private Dictionary<string, RCGameState> _states = new Dictionary<string, RCGameState>();
        private List<RCGameState> _stateStack = new List<RCGameState>();

        public event StateChangeFunc StateChanged;

        public RCGameStateManager(RCGame game)
            : base(game)
        {
            this.DrawOrder = 1;
            this.UpdateOrder = 1;
        }

        public void AddState(string label, Type stateType)
        {
            RCGameState state = RCGameStarter.BindTagObject<RCGameState>(label, stateType);
            _states.Add(label, state);
            state.Initialize();
        }

        public void RemoveState(string label)
        {
            RCGameState removeState = _states[label];
            removeState.Dispose();
            _states.Remove(label);
        }

        public void PushState(string label)
        {
            if (_stateStack.Count > 0)
            {
                if (StateChanged != null)
                {
                    StateChanged(_stateStack[0], _states[label]);
                }
            }
            _stateStack.Insert(0, _states[label]);
        }

        public RCGameState PopState()
        {
            if (_stateStack.Count == 0) return null;

            RCGameState oldState = _stateStack[0];
            _stateStack.RemoveAt(0);

            if (StateChanged != null)
            {
                if (_stateStack.Count >= 1)
                {
                    StateChanged(oldState, _stateStack[0]);
                }
            }

            return oldState;
        }

        public RCGameState PeekState()
        {
            if (_stateStack.Count == 0) return null;
            return _stateStack[0];
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = _stateStack.Count - 1; i >= 0; --i)
            {
                RCGameState currentState = _stateStack[i];

                if (!currentState.IsVisible) continue;

                currentState.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = _stateStack.Count - 1; i >= 0; --i)
            {
                RCGameState currentState = _stateStack[i];

                if (!currentState.IsUpdateable) continue;

                currentState.Update(gameTime);
            }

            base.Update(gameTime);
        }
        
        protected override void UnloadContent()
        {
            _states.Clear();
            base.UnloadContent();
        }
    }
}
