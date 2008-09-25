using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Rendering
{
    class RCEffectStateConfiguration 
    {
        Dictionary<Type, IRCEffectState> _effectStates = new Dictionary<Type, RCEffectState<EffectType>>();

        public RCEffectStateConfiguration(Type[] validStatetypes)
        {
            AddStateTypes(validStatetypes);
        }

        public void ApplyConfiguration()
        {
            // Apply each of the states individually.
            foreach (IRCEffectState state in _effectStates.Values)
            {
                if (state != null)
                {
                    state.ConfigureEffect();
                }
            }
        }

        private void AddStateTypes(Type[] validStatetypes)
        {
            foreach (Type t in validStatetypes)
            {
                _effectStates[types] = null;
            }
        }

        public void SetState(IRCEffectState state)
        {
            Type stateType = state.GetType();
            if (!_effectStates.ContainsKey())
            {
                throw new InvalidOperationException("Addded state is incompatible with effect.");
            }

            _effectStates[stateType] = state;
        }
    }
}
