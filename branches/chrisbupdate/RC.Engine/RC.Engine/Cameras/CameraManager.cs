using System;
using System.Collections.Generic;
using System.Text;

using RC.Engine.Rendering;

namespace RC.Engine.Cameras
{
    public interface IRCCameraManager : IDictionary<string, RCCamera>
    {
        RCCamera ActiveCamera { get; }

        void SetActiveCamera(string cameraLabel);
    }

    internal class RCCameraManager : IRCCameraManager
    {
        private Dictionary<string, RCCamera> cameras = new Dictionary<string, RCCamera>();
        private string activeCamera = String.Empty;

        public RCCamera ActiveCamera 
        { 
            get { return cameras[activeCamera]; } 
        }

        public void SetActiveCamera(string cameraLabel)
        {
            if (cameras.ContainsKey(cameraLabel))
            {
                activeCamera = cameraLabel;
            }
            else
            {
                activeCamera = String.Empty;
            }
        }

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
            get { return cameras[key]; }
            set { cameras[key] = value; }
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
}