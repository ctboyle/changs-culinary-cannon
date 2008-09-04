using System;
using System.Collections.Generic;
using System.Text;

using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Utility;

namespace RC.Engine.Cameras
{
    public interface IRCCameraManager : IRCLoadable
    {
        RCCamera ActiveCamera { get; }
        void AddCamera(string cameraLabel, RCCamera newCamera);
        void RemoveCamera(string cameraLabel);
        void SetActiveCamera(string cameraLabel);
        RCCamera this[string cameraLabel] { get; set ; }
    }

    public class RCCameraManager : IRCCameraManager
    {
        #region RCCameraDictionary

        internal class RCCameraDictionary : IDictionary<string, RCCamera>
        {
            private Dictionary<string, RCCamera> cameras = new Dictionary<string, RCCamera>();

            #region IDictionary<string,RCCamera> Members

            public void Add(string key, RCCamera value)
            {
                cameras.Add(key, value);
            }

            public bool ContainsKey(string key)
            {
                return cameras.ContainsKey(key);
            }

            public ICollection<string> Keys
            {
                get { return cameras.Keys; }
            }

            public bool Remove(string key)
            {
                return cameras.Remove(key);
            }

            public bool TryGetValue(string key, out RCCamera value)
            {
                return cameras.TryGetValue(key, out value);
            }

            public ICollection<RCCamera> Values
            {
                get { return cameras.Values; }
            }

            public RCCamera this[string key]
            {
                get
                {
                    return cameras[key];
                }
                set
                {
                    cameras[key] = value;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<string,RCCamera>> Members

            public void Add(KeyValuePair<string, RCCamera> item)
            {
                cameras.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                cameras.Clear();
            }

            public bool Contains(KeyValuePair<string, RCCamera> item)
            {
                return cameras.ContainsKey(item.Key) && cameras[item.Key].Equals(item.Value);
            }

            public void CopyTo(KeyValuePair<string, RCCamera>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return cameras.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(KeyValuePair<string, RCCamera> item)
            {
                return cameras.Remove(item.Key);
            }

            #endregion

            #region IEnumerable<KeyValuePair<string,RCCamera>> Members

            public IEnumerator<KeyValuePair<string, RCCamera>> GetEnumerator()
            {
                return cameras.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return cameras.GetEnumerator();
            }

            #endregion
        }

        #endregion

        private RCCameraDictionary cameras = new RCCameraDictionary();
        private string activeCameraLabel = String.Empty;
        private Game _game = null;

        public RCCameraManager(Game game)
        {
            _game = game;
            _game.Services.AddService(typeof(IRCCameraManager), this);
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        public RCCamera ActiveCamera { get { return cameras[activeCameraLabel]; } }

        public void AddCamera(string cameraLabel, RCCamera newCamera)
        {
            cameras.Add(cameraLabel, newCamera);
        }

        public void RemoveCamera(string cameraLabel)
        {
            cameras.Remove(cameraLabel);
        }

        public void SetActiveCamera(string cameraLabel)
        {
            activeCameraLabel = cameraLabel;
        }

        public RCCamera this[string cameraLabel]
        {
            get { return cameras[cameraLabel]; }
            set { cameras[cameraLabel] = value; }
        }
    }
}