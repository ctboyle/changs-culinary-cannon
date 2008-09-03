using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using RC.Engine.Rendering;

namespace RC.Engine.StateManagement
{
    public delegate void StateChangeHandler(RCGameState newState, RCGameState oldState);

    public interface IRCGameStateManager
    {
        event StateChangeHandler OnStateChange;

        RCGameState State { get; }
        
        void PopState();
        
        void PushState(RCGameState state);
        
        bool ContainsState(RCGameState state);
        
        void ChangeState(RCGameState newState);
    }

    internal class RCGameStateManager : DrawableGameComponent, IRCGameStateManager, IDisposable
    {
        private Stack<RCGameState> states = new Stack<RCGameState>();

        public event StateChangeHandler OnStateChange;

        public RCGameStateManager(Game game)
            : base(game)
        {
            Game.Components.Add(this);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Game.Components.Remove(this);
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            RCGameState state = states.Peek() as RCGameState;
            if (state != null)
            {
                state.Draw(gameTime, Game.Services);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            RCGameState state = states.Peek() as RCGameState;

            if (state != null)
            {
                state.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public void PopState()
        {
            RCGameState oldState = RemoveState();

            //Let everyone know we just changed states
            if (OnStateChange != null)
                OnStateChange(State, oldState);

            //Unregister the event for this state
            OnStateChange -= oldState.StateChanged;
        }

        public void PushState(RCGameState newState)
        {
            RCGameState oldState = State as RCGameState;
            AddState(newState);

            //Let everyone know we just changed states
            if (OnStateChange != null)
                OnStateChange(newState, oldState);
        }

        public void ChangeState(RCGameState newState)
        {
            //We are changing states, so pop everything ...
            //if we don't want to really change states but just modify,
            //we should call PushState and PopState
            while (states.Count > 0)
            {
                RCGameState oldState = RemoveState();
                if (oldState != null)
                {
                    //Unregister the event for this state
                    OnStateChange -= oldState.StateChanged;
                }
            }

            //changing state, reset our draw order
            AddState(newState);

            //Let everyone know we just changed states
            if (OnStateChange != null)
                OnStateChange(State, null);
        }

        public bool ContainsState(RCGameState state)
        {
            return (states.Contains(state));
        }

        public RCGameState State
        {
            get
            {
                if (states.Count != 0)
                {
                    return (states.Peek());
                }
                return null;
            }
        }

        private RCGameState RemoveState()
        {
            RCGameState oldState = (RCGameState)states.Peek();
            states.Pop();
            return oldState;
        }

        private void AddState(RCGameState state)
        {
            states.Push(state);

            //Register the event for this state
            OnStateChange += state.StateChanged;

            state.Initialize(Game.Services);
            state.LoadContent(Game.Content, Game.Services);
        }
    }
}