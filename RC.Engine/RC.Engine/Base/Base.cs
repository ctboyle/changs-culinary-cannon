using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Base
{
    public class RCBase
    {
        private RCGameContext _ctx;

        public RCBase(RCGameContext gameCtx)
        {
            _ctx = gameCtx;
        }

        protected RCGameContext Ctx
        {
            get { return _ctx; }
        }
    }
}