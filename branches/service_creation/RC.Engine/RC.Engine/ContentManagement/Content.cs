using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

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

    public class RCDefaultContent<T> : RCContent<T> where T : class
    {
        private string _assetName = string.Empty;

        public RCDefaultContent(IRCContentRequester contentRqst, string assetName)
            : base(contentRqst)
        {
            _assetName = assetName;
        }

        public RCDefaultContent(string assetName)
            : base()
        {
            _assetName = assetName;
        }

        public override object CreateType(IGraphicsDeviceService graphics, ContentManager content)
        {
            return content.Load<T>(_assetName);
        }
    }

    public abstract class RCContent<T> : IRCCreateType, IRCContent<T> where T : class
    {
        private Guid _id = Guid.Empty;
        private IRCContentManager _contentMgr = null;

        public RCContent()
        {
        }

        public RCContent(IRCContentRequester contentRqst)
        {
            Initialize(contentRqst);
        }

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

        public void Initialize(IRCContentRequester contentRqst)
        {
            contentRqst.RequestContent<T>(this, out _id, out _contentMgr);
            OnInitialize();
        }

        public bool IsInitialized 
        { 
            get { return (_contentMgr != null && _id != Guid.Empty); }
        }

        public abstract object CreateType(IGraphicsDeviceService graphics, ContentManager content);

        protected virtual void OnInitialize()
        {
        }
    }
}