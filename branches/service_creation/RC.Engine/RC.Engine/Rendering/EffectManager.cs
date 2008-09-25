using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using RC.Engine.Rendering;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RC.Engine.Utility;

    namespace RC.Engine.Effects
    {
        public interface IRCEffectManager : IRCLoadable
        {
            RCEffect ActiveEffect
            {
                get;
            }
            void AddEffect(string effectLabel, RCEffect newEffect);
            void RemoveEffect(string effectLabel);
            void SetActiveEffect(string effectLabel);
            RCEffect this[string effectLabel]
            {
                get;
                set;
            }
        }

        public class RCEffectManager : IRCEffectManager
        {
            #region RCEffectDictionary

            internal class RCEffectDictionary : IDictionary<string, RCEffect>
            {
                private Dictionary<string, RCEffect> _effects = new Dictionary<string, RCEffect>();

                #region IDictionary<string,RCEffect> Members

                public void Add(string key, RCEffect value)
                {
                    _effects.Add(key, value);
                }

                public bool ContainsKey(string key)
                {
                    return _effects.ContainsKey(key);
                }

                public ICollection<string> Keys
                {
                    get
                    {
                        return _effects.Keys;
                    }
                }

                public bool Remove(string key)
                {
                    return _effects.Remove(key);
                }

                public bool TryGetValue(string key, out RCEffect value)
                {
                    return _effects.TryGetValue(key, out value);
                }

                public ICollection<RCEffect> Values
                {
                    get
                    {
                        return _effects.Values;
                    }
                }

                public RCEffect this[string key]
                {
                    get
                    {
                        return _effects[key];
                    }
                    set
                    {
                        _effects[key] = value;
                    }
                }

                #endregion

                #region ICollection<KeyValuePair<string,RCEffect>> Members

                public void Add(KeyValuePair<string, RCEffect> item)
                {
                    _effects.Add(item.Key, item.Value);
                }

                public void Clear()
                {
                    _effects.Clear();
                }

                public bool Contains(KeyValuePair<string, RCEffect> item)
                {
                    return _effects.ContainsKey(item.Key) && _effects[item.Key].Equals(item.Value);
                }

                public void CopyTo(KeyValuePair<string, RCEffect>[] array, int arrayIndex)
                {
                    throw new NotImplementedException();
                }

                public int Count
                {
                    get
                    {
                        return _effects.Count;
                    }
                }

                public bool IsReadOnly
                {
                    get
                    {
                        return false;
                    }
                }

                public bool Remove(KeyValuePair<string, RCEffect> item)
                {
                    return _effects.Remove(item.Key);
                }

                #endregion

                #region IEnumerable<KeyValuePair<string,RCEffect>> Members

                public IEnumerator<KeyValuePair<string, RCEffect>> GetEnumerator()
                {
                    return _effects.GetEnumerator();
                }

                #endregion

                #region IEnumerable Members

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return _effects.GetEnumerator();
                }

                #endregion
            }

            #endregion

            private RCEffectDictionary _effects = new RCEffectDizctionary();
            private string _activeEffectLabel = String.Empty;
            private Game _game = null;

            public RCEffectManager(Game game)
            {
                _game = game;
                _game.Services.AddService(typeof(IRCEffectManager), this);
            }

            public void Load()
            {
            }

            public void Unload()
            {
            }

            public RCEffect ActiveEffect
            {
                get
                {
                    return _effects[_activeEffectLabel];
                }
            }

            public void AddEffect(string effectLabel, RCEffect newEffect)
            {
                _effects.Add(effectLabel, newEffect);
            }

            public void RemoveEffect(string effectLabel)
            {
                _effects.Remove(effectLabel);
            }

            public void SetActiveEffect(string effectLabel)
            {
                _activeEffectLabel = effectLabel;
            }

            public RCEffect this[string effectLabel]
            {
                get
                {
                    return _effects[effectLabel];
                }
                set
                {
                    _effects[effectLabel] = value;
                }
            }
        }
    }
}
