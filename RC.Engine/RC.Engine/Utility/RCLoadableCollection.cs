using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Utility
{
    internal class RCLoadableCollection : ICollection<IRCLoadable>
    {
        private List<IRCLoadable> mgrs = new List<IRCLoadable>();

        #region ICollection<IRCManager> Members

        public void Add(IRCLoadable item)
        {
            mgrs.Add(item);   
        }

        public void Clear()
        {
            mgrs.Clear();
        }

        public bool Contains(IRCLoadable item)
        {
            return mgrs.Contains(item);
        }

        public void CopyTo(IRCLoadable[] array, int arrayIndex)
        {
            mgrs.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return mgrs.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IRCLoadable item)
        {
            return mgrs.Remove(item);
        }

        #endregion

        #region IEnumerable<IRCManager> Members

        public IEnumerator<IRCLoadable> GetEnumerator()
        {
            return mgrs.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mgrs.GetEnumerator();
        }

        #endregion
    }
}