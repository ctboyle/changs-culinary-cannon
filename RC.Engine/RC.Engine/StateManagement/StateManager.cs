using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;

namespace RC.Engine.StateManagement
{
    public delegate void StateChangeHandler(RCGameState newState, RCGameState oldState);

    public interface IRCGameStateManager
    {
        bool IsLoaded { get; set; }
        void AddState(string label, RCGameState state);
        void RemoveState(string label);
        void PushState(string label);
        RCGameState PopState();
        RCGameState PeekState();
    }

    internal class RCGameStateManager : DrawableGameComponent, IRCGameStateManager, IDisposable
    {
        public delegate void StateChangeFunc(RCGameState previousState, RCGameState newState);

        private Dictionary<string, RCGameState> states = new Dictionary<string, RCGameState>();
        private List<RCGameState> stateStack = new List<RCGameState>();
        private bool _isLoaded = false;

        public event StateChangeFunc StateChanged;

        public RCGameStateManager(Game game)
            : base(game)
        {
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

            states.Add(label, state);
        }

        public void RemoveState(string label)
        {
            if (_isLoaded)
            {
                throw new InvalidOperationException("States cannot be removed after content is loaded.");
            }

            states.Remove(label);
        }

        public void PushState(string label)
        {
            if (stateStack.Count > 0)
            {
                if (StateChanged != null)
                {
                    StateChanged(stateStack[0], states[label]);
                }
            }

            stateStack.Insert(0, states[label]);
        }

        public RCGameState PopState()
        {
            if (stateStack.Count == 0) return null;

            RCGameState oldState = stateStack[0];
            stateStack.RemoveAt(0);

            if (StateChanged != null)
            {
                if (stateStack.Count >= 1)
                {
                    StateChanged(oldState, stateStack[0]);
                }
            }

            return oldState;
        }

        public RCGameState PeekState()
        {
            if (stateStack.Count == 0) return null;
            return stateStack[0];
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = stateStack.Count - 1; i >= 0; --i)
            {
                RCGameState currentState = stateStack[i];

                if (!currentState.IsVisible) continue;

                currentState.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = stateStack.Count - 1; i >= 0; --i)
            {
                RCGameState currentState = stateStack[i];

                if (!currentState.IsUpdateable) continue;

                currentState.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            foreach (RCGameState state in states.Values)
            {
                state.Load();
            }

            _isLoaded = true;

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (RCGameState state in states.Values)
            {
                state.Unload();
            }

            _isLoaded = false;
            states.Clear();

            base.UnloadContent();
        }
    }
}