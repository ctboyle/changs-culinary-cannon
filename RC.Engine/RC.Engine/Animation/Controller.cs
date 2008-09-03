using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Animation
{
    public interface IRCController
    {
        bool IsAnimating { get; }
        void Update(GameTime gameTime);
        RCSpatial Parent { get; }
    }
  
    public abstract class RCController<CntrlType> 
        : IRCController 
            where CntrlType : RCSpatial
    {
        public delegate void AnimationCompleteHandler();
        public event AnimationCompleteHandler OnComplete;

        protected CntrlType _controlledItem;
        protected bool _isAnimating;
        
        public RCSpatial Parent
        {
            get { return _controlledItem; }
        }

        public bool IsAnimating 
        {
            get { return _isAnimating; }
        }

        public bool AttachToObject(CntrlType parent)
        {
            bool fSuccess = false;
            if (parent != null)
            {
                _controlledItem = parent;
                fSuccess = _controlledItem.AddController(this);
            }

           
            return fSuccess;
        }


        public RCController()
        {
            _controlledItem = null;
            _isAnimating = false;
        }

        public abstract void Update(GameTime gameTime);

        protected void FireAnimationComplete()
        {
            if (OnComplete != null)
            {
                OnComplete();
            }
        }
    }
}