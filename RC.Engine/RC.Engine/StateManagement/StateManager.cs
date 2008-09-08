using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;

namespace RC.Engine.StateManagement
{
    public delegate void StateChangeHandler(RCGameState newState, RCGameState oldState);

    public interface IRCGameStateStack
    {
        void PushState(string label);
        RCGameState PopState();
        RCGameState PeekState();
    }

    public interface IRCGameStateManager : IRCGameStateStack
    {
        bool IsLoaded { get; set; }
        void AddState(string label, RCGameState state);
        void RemoveState(string label);
    }

    internal class RCGameStateManager : DrawableGameComponent, IRCGameStateManager
    {
        public delegate void StateChangeFunc(RCGameState previousState, RCGameState newState);

        private Dictionary<string, RCGameState> _states = new Dictionary<string, RCGameState>();
        private List<RCGameState> _stateStack = new List<RCGameState>();
        private bool _isLoaded = false;

        public event StateChangeFunc StateChanged;

        public RCGameStateManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IRCGameStateStack), this);
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        public void AddState(string label, RCGameState state)
        {
            if (_isLoaded)
            {
                throw new InvalidOperationException("States cannot be added after content is loaded.");
            }

            _states.Add(label, state);
        }

        public void RemoveState(string label)
        {
            if (_isLoaded)
            {
                throw new InvalidOperationException("States cannot be removed after content is loaded.");
            }

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

        protected override void LoadContent()
        {
            foreach (RCGameState state in _states.Values)
            {
                state.Load();
            }

            _isLoaded = true;

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (RCGameState state in _states.Values)
            {
                state.Unload();
            }

            _isLoaded = false;
            _states.Clear();

            base.UnloadContent();
        }
    }
}