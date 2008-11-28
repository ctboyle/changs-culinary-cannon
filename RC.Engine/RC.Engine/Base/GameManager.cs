using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Base;
using RC.Engine.Plugin;

namespace RC.Engine.Base
{
    public interface RCGameManager
    {
        void Start(Type gameType);
        void LoadModule(IRCModule module);
        T RegisterBaseType<T>(String tag, Type baseType) where T : RCBase;
    }
}