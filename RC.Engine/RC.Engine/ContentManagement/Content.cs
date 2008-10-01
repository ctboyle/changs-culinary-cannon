using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.ContentManagement
{
    public class RCContent<T> : IDisposable where T : IDisposable 
    {
        private Guid _id = Guid.Empty;
        private IRCContentManager _contentMgr = null;

        internal RCContent(Guid id, IRCContentManager contentMgr)
        {
            _id = id;
            _contentMgr = contentMgr;
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
            _contentMgr.UnloadContent(_id);
            GC.SuppressFinalize(this);
        }

        #endregion
        
        public T Content
        {
            get { return _contentMgr.LoadContent<T>(_id); }
        }
    }
}