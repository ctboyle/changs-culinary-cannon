using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Rendering
{
    public class RCRenderStateCollection : IEnumerable<RCRenderState>
    {
        public Dictionary<RCRenderState.StateType, RCRenderState> _states = new 
            Dictionary<RCRenderState.StateType, RCRenderState>();

        public RCRenderStateCollection(bool fInitializeDefualtStates)
        {
            if (fInitializeDefualtStates)
            {
                // Iterate through the defaut states and add them to the collection.
                foreach (RCRenderState state in RCRenderState.Default)
                {
                    Add(state);
                }
            }
        }

        public void Add(RCRenderState item)
        {
            _states[item.GetStateType()] = item;
        }

        public void Clear()
        {
            _states.Clear();
        }

        public bool Contains(RCRenderState.StateType itemType)
        {
            return _states.ContainsKey(itemType);
        }

        public int Count
        {
            get
            {
                return _states.Count;
            }
        }

        public RCRenderState this[RCRenderState.StateType type]
        {
            get
            {
                RCRenderState state = null;

                _states.TryGetValue(type, out state);

                return state;
                
            }
            set
            {
                _states[type] = value;
            }
        }

        public bool Remove(RCRenderState.StateType itemType)
        {
            return _states.Remove(itemType);
        }

        #region IEnumerable<RCRenderState> Members

        public IEnumerator<RCRenderState> GetEnumerator()
        {
            return _states.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _states.Values.GetEnumerator();
        }

        #endregion
    }
}
