using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.ContentManagement
{
    /// <summary>
    /// Defines functionality to create the underlying content type.
    /// </summary>
    interface IRCCreateType
    {
        /// <summary>
        /// Create the underlying content type.
        /// </summary>
        /// <param name="graphics">The graphics service.</param>
        /// <param name="content">The content pipeline loader.</param>
        /// <returns>The underlying content type.</returns>
        object CreateType(IGraphicsDeviceService graphics, ContentManager content);

        void OnFinishedLoad();
    }

    /// <summary>
    /// The content loaded from the IRCContentManager.
    /// </summary>
    /// <typeparam name="T">The underlying content type.</typeparam>
    interface IRCContent<T> : IDisposable where T : class
    {
        /// <summary>
        /// The loaded content.
        /// </summary>
        T Content { get; }
        
        /// <summary>
        /// Initializes the underlying content.
        /// </summary>
        /// <param name="contentRqst">The content requester (if needed).</param>
        void Initialize(IRCContentRequester contentRqst);

        /// <summary>
        /// Returns if the content is initialized.
        /// </summary>
        bool IsInitialized { get; }
    }

    /// <summary>
    /// Helper class that allows content to be loaded by asset name.
    /// </summary>
    /// <typeparam name="T">The real type.</typeparam>
    public class RCDefaultContent<T> : RCContent<T> where T : class
    {
        private string _assetName = string.Empty;

        /// <summary>
        /// Creates a new content instance.
        /// </summary>
        /// <param name="assetName">The asset name.</param>
        public RCDefaultContent(string assetName)
            : base()
        {
            _assetName = assetName;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copy">The copy constructor.</param>
        public RCDefaultContent(RCContent<T> copy)
            : base(copy)
        {
        }

        /// <summary>
        /// Creates the content type.
        /// </summary>
        /// <param name="graphics">The graphics device service.</param>
        /// <param name="content">The content manager.</param>
        /// <returns>The created content type.</returns>
        protected override object OnCreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            return content.Load<T>(_assetName);
        }
    }

    /// <summary>
    /// Content that will be allocated through <see cref="RCContentManager"/>
    /// </summary>
    /// <typeparam name="T">The content type.</typeparam>
    public abstract class RCContent<T> : IRCCreateType, IRCContent<T> where T : class
    {
        private Guid _id = Guid.Empty;

        /// <summary>
        /// The content manager.
        /// </summary>
        protected IRCContentManager _contentMgr = null;

        /// <summary>
        /// Implicitly casts to the underlying type.
        /// </summary>
        /// <param name="theContent">The content.</param>
        /// <returns>The underlying type.</returns>
        public static implicit operator T(RCContent<T> theContent)
        {
            return theContent.Content;
        }

        /// <summary>
        /// Creates a new instance of the content.
        /// </summary>
        public RCContent()
        {
            Initialize(RCContentManager.ActiveRequester);
        }

        /// <summary>
        /// Copy constructor for the content.
        /// </summary>
        /// <param name="copy">The content to copy from.</param>
        public RCContent(RCContent<T> copy)
        {
            _id = copy._id;
            _contentMgr = copy._contentMgr;
        }

        ~RCContent()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_contentMgr != null)
            {
                _contentMgr.UnloadContent(_id);
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// The content.
        /// </summary>
        public T Content
        {
            get
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("The content has not been initialized.");
                }

                return _contentMgr.LoadContent<T>(_id);
            }
        }

        /// <summary>
        /// Initializes the content with the content requester.
        /// </summary>
        /// <param name="contentRqst">The content requester.</param>
        public void Initialize(IRCContentRequester contentRqst)
        {
            contentRqst.RequestContent<T>(this, out _id, out _contentMgr);
            OnInitialize();
        }

        /// <summary>
        /// Returns if initialized with the content manager.
        /// </summary>
        public bool IsInitialized 
        { 
            get { return (_contentMgr != null && _id != Guid.Empty); }
        }

        /// <summary>
        /// Creates the underlying type.
        /// </summary>
        /// <param name="graphics">The graphics device service.</param>
        /// <param name="content">The content manager.</param>
        /// <returns>The underlying type instance.</returns>
        protected abstract object OnCreateType(IGraphicsDeviceService graphics, ContentManager content);

        /// <summary>
        /// Creates the underlying type.
        /// </summary>
        /// <param name="graphics">The graphics device service.</param>
        /// <param name="content">The content manager.</param>
        /// <returns>The underlying type instance.</returns>
        public object CreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            object createdType = OnCreateType(graphics, content);
            return createdType;
        }

        /// <summary>
        /// Called during initialization.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Do any initialzaion of the loaded contenet here.
        /// </summary>
        public virtual void OnFinishedLoad()
        {

        }
    }
}