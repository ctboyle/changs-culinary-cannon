using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Base;

namespace RC.Engine.Cameras
{
    /// <summary>
    /// I am the camera mananager.  I keep a set of cameras and maintain
    /// the active camera.
    /// </summary>
    /// <remarks>
    /// The active camera is used by the <see cref="IRCRenderManager"/> to render any scenes drawn at the time.
    /// Be sure to set the correct active camera before rendering a scene.
    /// </remarks>
    public interface IRCCameraManager
    {
        /// <summary>
        /// The active camera.
        /// </summary>
        RCCamera ActiveCamera { get; }

        /// <summary>
        /// I add a new camera to the set of cameras.
        /// </summary>
        /// <param name="cameraLabel">The camera label.</param>
        /// <param name="newCamera">The camera instance.</param>
        void AddCamera(string cameraLabel, RCCamera newCamera);

        /// <summary>
        /// I remove from the set of cameras by label.
        /// </summary>
        /// <param name="cameraLabel">The camera label.</param>
        void RemoveCamera(string cameraLabel);

        /// <summary>
        /// I set the active camera by label.
        /// </summary>
        /// <param name="cameraLabel">The camera label.</param>
        void SetActiveCamera(string cameraLabel);

        /// <summary>
        /// I give access to the cameras by label.
        /// </summary>
        /// <param name="cameraLabel">The camera label.</param>
        /// <returns>The camera.</returns>
        RCCamera this[string cameraLabel] { get; set ; }
    }

    internal class RCCameraManager : IRCCameraManager
    {
        #region RCCameraDictionary

        internal class RCCameraDictionary : IDictionary<string, RCCamera>
        {
            private Dictionary<string, RCCamera> _cameras = new Dictionary<string, RCCamera>();

            #region IDictionary<string,RCCamera> Members

            public void Add(string key, RCCamera value)
            {
                _cameras.Add(key, value);
            }

            public bool ContainsKey(string key)
            {
                return _cameras.ContainsKey(key);
            }

            public ICollection<string> Keys
            {
                get { return _cameras.Keys; }
            }

            public bool Remove(string key)
            {
                return _cameras.Remove(key);
            }

            public bool TryGetValue(string key, out RCCamera value)
            {
                return _cameras.TryGetValue(key, out value);
            }

            public ICollection<RCCamera> Values
            {
                get { return _cameras.Values; }
            }

            public RCCamera this[string key]
            {
                get
                {
                    return _cameras[key];
                }
                set
                {
                    _cameras[key] = value;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<string,RCCamera>> Members

            public void Add(KeyValuePair<string, RCCamera> item)
            {
                _cameras.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                _cameras.Clear();
            }

            public bool Contains(KeyValuePair<string, RCCamera> item)
            {
                return _cameras.ContainsKey(item.Key) && _cameras[item.Key].Equals(item.Value);
            }

            public void CopyTo(KeyValuePair<string, RCCamera>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return _cameras.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(KeyValuePair<string, RCCamera> item)
            {
                return _cameras.Remove(item.Key);
            }

            #endregion

            #region IEnumerable<KeyValuePair<string,RCCamera>> Members

            public IEnumerator<KeyValuePair<string, RCCamera>> GetEnumerator()
            {
                return _cameras.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _cameras.GetEnumerator();
            }

            #endregion
        }

        #endregion

        private RCCameraDictionary _cameras = new RCCameraDictionary();
        private string _activeCameraLabel = String.Empty;

        public RCCamera ActiveCamera { get { return _cameras[_activeCameraLabel]; } }

        public RCCameraManager(Game game)
        {
            game.Services.AddService(typeof(IRCCameraManager), this);
        }

        public void AddCamera(string cameraLabel, RCCamera newCamera)
        {
            _cameras.Add(cameraLabel, newCamera);
        }

        public void RemoveCamera(string cameraLabel)
        {
            _cameras.Remove(cameraLabel);
        }

        public void SetActiveCamera(string cameraLabel)
        {
            _activeCameraLabel = cameraLabel;
        }

        public RCCamera this[string cameraLabel]
        {
            get { return _cameras[cameraLabel]; }
            set { _cameras[cameraLabel] = value; }
        }
    }
}