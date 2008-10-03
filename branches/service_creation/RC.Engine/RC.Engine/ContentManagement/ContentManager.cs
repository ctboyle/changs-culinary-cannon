using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Ninject.Core;

namespace RC.Engine.ContentManagement
{
    /// <summary>
    /// Provides functionality to request content.
    /// </summary>
    public interface IRCContentRequester : IGameComponent
    {
        /// <summary>
        /// Request the content.
        /// </summary>
        /// <typeparam name="T">The expected object type.</typeparam>
        /// <param name="content">The content object.</param>
        /// <returns>The </returns>
        void RequestContent<T>(
            RCContent<T> content
            )
            where T : class, IDisposable;
    }

    /// <summary>
    /// Provides functionality to load and unload content directly.
    /// </summary>
    public interface IRCContentManager : IRCContentRequester
    {
        /// <summary>
        /// Loads content by id.
        /// </summary>
        /// <typeparam name="T">The expected object type.</typeparam>
        /// <param name="id">The content id.</param>
        /// <returns>The content.</returns>
        T LoadContent<T>(
            Guid id
            ) 
            where T : class,IDisposable;

        /// <summary>
        /// Unloads content by id.
        /// </summary>
        /// <param name="id">The content id.</param>
        void UnloadContent(
            Guid id
            );
    }

    /// <summary>
    /// The content manager that maintains all content.  Maintains an accurate list 
    /// of content to be loaded.  If exception occur when all content may need to 
    /// reload and unload, the content manager will take responsibility.
    /// </summary>
    [Singleton]
    internal class RCContentManager : DrawableGameComponent, IRCContentManager, IDisposable
    {
        private class ContentStore
        {
            public IDisposable Content;
            public IRCCreateType TypeCreator;
        }

        /// <summary>
        /// The content stored by id.
        /// </summary>
        private Dictionary<Guid, ContentStore> _content = new Dictionary<Guid, ContentStore>();

        /// <summary>
        /// The graphics device manager.
        /// </summary>
        private IGraphicsDeviceService _graphics = null;
        
        /// <summary>
        /// The content pipeline content manager.
        /// </summary>
        private ContentManager _contentMgr = null;

        public RCContentManager(RCBasicGame game)
            : base(game)
        {
        }

        ~RCContentManager()
        {
            Dispose();
        }

        [Inject]
        public ContentManager ContentMgr
        {
            get { return _contentMgr; }
            set { _contentMgr = value; }
        }

        [Inject]
        public IGraphicsDeviceService Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        public void RequestContent<T>(
            RCContent<T> content
            )
            where T : class,IDisposable
        {
            Guid id = this.CreateNewId();

            // Setup the content storage
            ContentStore store = new ContentStore();
            store.Content = null;
            store.TypeCreator = content;
            _content.Add(id, store);

            // Setup the content object
            content.Init(id, this);
        }

        public T LoadContent<T>(
            Guid id
            ) 
            where T : class,IDisposable
        {
            T content = (T)_content[id].Content;

            // If the content loaded for the given id is
            // null, then we must try to load it
            if (content == null)
            {
                content = (T)_content[id].TypeCreator.CreateType(Graphics, ContentMgr);
                _content[id].Content = content;
            }

            return content;
        }

        public void UnloadContent(
            Guid id
            )
        {
            // Get the content and try to dispose it, in order
            // to immediately release the resources.
            IDisposable content = _content[id].Content;
            if (content != null && content.GetType() != typeof(Effect))
                content.Dispose();
            
            // Remove the id from our lists
            _content.Remove(id);
        }

        public override void Initialize()
        {
            UpdateOrder = 0;
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            //TODO: It would be nice if we could use the UnloadContent(Guid id) fn.

            // Get a list of all content and dispose it.
            ContentStore[] disposeMe = new ContentStore[_content.Values.Count];
            _content.Values.CopyTo(disposeMe, 0);
            foreach (ContentStore disposable in disposeMe)
            {
                if (disposable.Content == null || disposable.Content.GetType() == typeof(Effect)) continue;
                disposable.Content.Dispose();
            }

            // Set all of the content to null.
            Guid[] guids = new Guid[_content.Keys.Count];
            _content.Keys.CopyTo(guids, 0);
            foreach (Guid id in guids)
            {
                _content[id].Content = null;
            }

            base.UnloadContent();
        }

        private Guid CreateNewId()
        {
            Guid id = Guid.NewGuid();
            if (_content.ContainsKey(id))
            {
                return CreateNewId();
            }
            return id;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            UnloadContent();
        }

        #endregion
    }
}