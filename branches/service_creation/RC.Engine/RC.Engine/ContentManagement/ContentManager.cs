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
    /// RCAllocateRequestFn is a delegate that actually allocates a content
    /// resource that needs to be managed.  This delegate exists for the 
    /// instances that the XNA ContentManager will not be able to explicitly
    /// load using the asset name.  
    /// 
    /// <example>
    /// For example, the allocation of BasicEffect cannot be done using an asset name. 
    /// In this case a the following can be defined:
    /// </example>
    ///
    /// <code>
    /// delegate(params object[] parameters) { new BasicEffect(parameters[0], null); }
    /// </code>
    /// 
    /// </summary>
    /// <param name="parameters">Required parameters for the delegate instance.</param>
    /// <returns>An allocated object that needs to be managed.</returns>
    public delegate object RCAllocateRequestFn(params object[] parameters);

    /// <summary>
    /// RCWrapperRequestFn is a delegate that will allow an allocated object to
    /// be used in a wrapper instance of RCContent.  If an instance of RCContent
    /// has additional constructor parameters or setup, the managed object will
    /// be assigned and given an unique id, which will be provided to the delegate
    /// instance for the creation of the wrapper.  
    /// 
    /// <example>
    /// For example, RCTextureEffect requires both a Texture2D and a Effect that 
    /// needs to be managed.  In order to allow this, the texture content should 
    /// be loaded first and then provided to RCTextureEffect through this delegate 
    /// instance. The effect itself will still be allocated using its asset name,
    /// but the texture will also be provided.
    /// </example>
    /// 
    /// <code>
    /// delegate(Guid id, IRCContentManager c) { 
    /// return new RCTextureEffect(id, cont, texture); 
    /// }
    /// </code>
    /// 
    /// </summary>
    /// <param name="id">The unique id of the managed resource.</param>
    /// <param name="contentMgr">The content manager.</param>
    /// <returns>The wrapper instance of the content.</returns>
    public delegate object RCWrapperRequestFn(Guid id, IRCContentManager contentMgr);
        
    /// <summary>
    /// The content requester is used to request content to be loaded through either the
    /// content pipeline or through any other custome means.
    /// </summary>
    public interface IRCContentRequester : IGameComponent
    {
        /// <summary>
        /// Will request content to be loaded.
        /// </summary>
        /// <typeparam name="T">The content type that is loaded.</typeparam>
        /// <param name="request">The wrapper request provider.</param>
        /// <param name="customRequest">The actual provider.</param>
        /// <param name="parameters">Any parameters needed for <paramref name="customRequest"/></param>
        /// <returns>The content.</returns>
        RCContent<T> RequestContent<T>(
            RCWrapperRequestFn request, 
            RCAllocateRequestFn customRequest, 
            params object[] parameters
            ) 
            where T : IDisposable;

        /// <summary>
        /// Will request content to be loaded.
        /// </summary>
        /// <typeparam name="T">The content type that is loaded.</typeparam>
        /// <param name="customRequest">The actual provider.</param>
        /// <param name="parameters">Any parameters needed for <paramref name="customRequest"/></param>
        /// <returns>The content.</returns>
        RCContent<T> RequestContent<T>(
            RCAllocateRequestFn customRequest, 
            params object[] parameters
            ) 
            where T : IDisposable;

        /// <summary>
        /// Will request content to be loaded.
        /// </summary>
        /// <typeparam name="T">The content type that is loaded.</typeparam>
        /// <param name="request">The wrapper request provider.</param>
        /// <param name="assetName">The asset name for the content pipeline.</param>
        /// <returns>The content.</returns>
        RCContent<T> RequestContent<T>(
            RCWrapperRequestFn request, 
            string assetName
            ) 
            where T : IDisposable;

        /// <summary>
        /// Will request content to be loaded.
        /// </summary>
        /// <typeparam name="T">The content type that is loaded.</typeparam>
        /// <param name="assetName">The asset name for the content pipeline.</param>
        /// <returns>The content.</returns>
        RCContent<T> RequestContent<T>(
            string assetName
            ) 
            where T : IDisposable;
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
        T LoadContent<T>(Guid id) where T : IDisposable;

        /// <summary>
        /// Unloads content by id.
        /// </summary>
        /// <param name="id">The content id.</param>
        void UnloadContent(Guid id);
    }

    /// <summary>
    /// The content manager that maintains all content.  Maintains an accurate list 
    /// of content to be loaded.  If exception occur when all content may need to 
    /// reload and unload, the content manager will take responsibility.
    /// </summary>
    [Singleton]
    internal class RCContentManager : DrawableGameComponent, IRCContentManager, IDisposable
    {
        /// <summary>
        /// The parameters needed for the object allocation request delegates.
        /// </summary>
        private Dictionary<Guid, List<object>> _objectParams = new Dictionary<Guid, List<object>>();
        
        /// <summary>
        /// The delegates needed to allocate the content.
        /// </summary>
        private Dictionary<Guid, RCAllocateRequestFn> _objectRequests = new Dictionary<Guid, RCAllocateRequestFn>();
        
        /// <summary>
        /// The content stored by id.
        /// </summary>
        private Dictionary<Guid, IDisposable> _content = new Dictionary<Guid, IDisposable>();
        
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
        
        public RCContent<T> RequestContent<T>(
            RCWrapperRequestFn request, 
            RCAllocateRequestFn customRequest, 
            params object[] parameters
            ) 
            where T : IDisposable
        {
            Guid id = SetupContentRequest<T>(customRequest, parameters);
            return (RCContent<T>) request(id, this);
        }

        public RCContent<T> RequestContent<T>(
            RCAllocateRequestFn customRequest, 
            params object[] parameters
            ) 
            where T : IDisposable
        {
            Guid id = SetupContentRequest<T>(customRequest, parameters);
            return new RCContent<T>(id, this);
        }

        public RCContent<T> RequestContent<T>(
            RCWrapperRequestFn request, 
            string assetName
            ) 
            where T : IDisposable
        {
            Guid id = SetupDefaultContentRequest<T>(assetName);
            return (RCContent<T>) request(id, this);
        }

        public RCContent<T> RequestContent<T>(
            string assetName
            ) 
            where T : IDisposable
        {
            Guid id = SetupDefaultContentRequest<T>(assetName);
            return new RCContent<T>(id, this);
        }

        public T LoadContent<T>(
            Guid id
            ) 
            where T : IDisposable
        {
            T content = (T)_content[id];

            // If the content loaded for the given id is
            // null, then we must try to load it
            if (content == null)
            {
                object[] parameters = _objectParams[id].ToArray();
                RCAllocateRequestFn request = _objectRequests[id];
                content = (T)request(parameters);
                _content[id] = content;
            }

            return content;
        }

        public void UnloadContent(
            Guid id
            )
        {
            // Get the content and try to dispose it, in order
            // to immediately release the resources.
            IDisposable content = _content[id];
            if (content != null && content.GetType() != typeof(Effect))
                content.Dispose();
            
            // Remove the id from our lists
            _objectParams.Remove(id);
            _objectRequests.Remove(id);
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
            IDisposable[] disposeMe = new IDisposable[_content.Values.Count];
            _content.Values.CopyTo(disposeMe, 0);
            foreach (IDisposable disposable in disposeMe)
            {
                if (disposable == null || disposable.GetType() == typeof(Effect)) continue;
                disposable.Dispose();
            }

            // Set all of the content to null.
            Guid[] guids = new Guid[_content.Keys.Count];
            _content.Keys.CopyTo(guids, 0);
            foreach (Guid id in guids)
            {
                _content[id] = null;
            }

            base.UnloadContent();
        }

        private object DefaultRequest<T>(
            params object[] parameters
            ) 
            where T : IDisposable
        {
            string assetName = parameters[0] as string;
            return ContentMgr.Load<T>(assetName);
        }

        private Guid SetupDefaultContentRequest<T>(
            string assetName
            ) 
            where T : IDisposable
        {
            return SetupContentRequest<T>(DefaultRequest<T>, assetName);
        }

        private Guid SetupContentRequest<T>(
            RCAllocateRequestFn request, 
            params object[] parameters
            ) 
            where T : IDisposable
        {
            Guid id = CreateNewId();
            SetupFactory<T>(id, request, parameters);
            _content.Add(id, null);
            return id;
        }

        private void SetupFactory<T>(
            Guid id, 
            RCAllocateRequestFn request, 
            params object[] parameters
            ) 
            where T : IDisposable
        {
            // Set the parameters for the factory delegate
            _objectParams.Add(id, new List<object>());
            if (parameters != null)
            {
                _objectParams[id].AddRange(parameters);
            }

            // Set the factory delegate
            _objectRequests.Add(id, request);
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