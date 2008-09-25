using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Rendering
{
    public class RCRenderStateStack
    {
        private Dictionary<RCRenderState.StateType, Stack<RCRenderState>> _stateStacks 
            = new Dictionary<RCRenderState.StateType,Stack<RCRenderState>>();


        public RCRenderStateStack()
        {
            foreach (RCRenderState.StateType type in Enum.GetValues(typeof(RCRenderState.StateType)))
            {
                _stateStacks[type] = new Stack<RCRenderState>();
            }

            // Initialize with defaults;
            PushStates(RCRenderState.Default);
        }

        /// <summary>
        /// Takes all the states in the collection and pushes them individualy on the
        /// appropriate stack.
        /// </summary>
        /// <param name="states"></param>
        public void PushStates(RCRenderStateCollection states)
        {
            foreach (RCRenderState state in states)
            {
                RCRenderState.StateType type = state.GetStateType();
                _stateStacks[type].Push(state);
            }
        }

        public void PopStates(RCRenderStateCollection states)
        {
            foreach (RCRenderState state in states)
            {
                RCRenderState.StateType type = state.GetStateType();
                _stateStacks[type].Pop();
                // Do not care about keeping the popped state.
            }
        }

        /// <summary>
        /// Peek at the top most state on the stack specified by type.
        /// </summary>
        /// <param name="type">The stack to retrive the top most state.</param>
        /// <returns></returns>
        public RCRenderState this[RCRenderState.StateType type]
        {
            get
            {
                return _stateStacks[type].Peek();
            }            
        }
        
    }
}
