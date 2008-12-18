using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using RC.Engine.ContentManagement;
using RC.Engine.Base;

namespace RC.Engine.StateManagement
{
    public delegate void StateChangeHandler(RCGameState newState, RCGameState oldState);

    /// <summary>
    /// I am the game state stack and maintain the current stack.  I have 
    /// the ability to push, pop, and peek states from the stack.
    /// </summary>
    public interface IRCGameStateStack
    {
        /// <summary>
        /// I push states onto the stack by name.
        /// </summary>
        /// <param name="label">The state name.</param>
        void PushState(string label);

        /// <summary>
        /// I pop the top state off the stack.
        /// </summary>
        /// <returns>The state.</returns>
        RCGameState PopState();

        /// <summary>
        /// I peek the top state on the stack.  I do not alter the stack.
        /// </summary>
        /// <returns>The state.</returns>
        RCGameState PeekState();
    }
    
    /// <summary>
    /// I am the game state manager and maintain the current stack.  I have 
    /// the ability to push, pop, and peek states from the stack.
    /// Because states are not pushed, poped, etc. directly
    /// but instead by a String name, I control the pool of states are 
    /// used with the stack.
    /// </summary>
    public interface IRCGameStateManager : IGameComponent
    {
        /// <summary>
        /// I inform that a state has changed.
        /// </summary>
        event StateChangeHandler StateChanged;

        /// <summary>
        /// I add a state to the pool of states by name.
        /// </summary>
        /// <param name="label">The state name.</param>
        /// <param name="state">The state.</param>
        void AddState(string label, RCGameState state);

        /// <summary>
        /// I remove a state from the pool of states by name.
        /// </summary>
        /// <param name="label">The state name.</param>
        void RemoveState(string label);
    }

    internal class RCGameStateManager : DrawableGameComponent, IRCGameStateManager, IRCGameStateStack
    {
        private Dictionary<string, RCGameState> _states = new Dictionary<string, RCGameState>();
        private List<RCGameState> _stateStack = new List<RCGameState>();

        public RCGameStateManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IRCGameStateManager), this);
            game.Services.AddService(typeof(IRCGameStateStack), this);

            this.DrawOrder = 1;
            this.UpdateOrder = 1;
        }

        public event StateChangeHandler StateChanged;

        public void AddState(string label, RCGameState state)
        {
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
            _stateStack.Insert(0, _states[label]);

            if (StateChanged != null)
            {
                if (_stateStack.Count > 1)
                {
                    StateChanged(_stateStack[0], _stateStack[1]);
                }
                else
                {
                    StateChanged(_stateStack[0], null);
                }   
            }
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
                    StateChanged(_stateStack[0], oldState);
                }
                else
                {
                    StateChanged(null, oldState);
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
