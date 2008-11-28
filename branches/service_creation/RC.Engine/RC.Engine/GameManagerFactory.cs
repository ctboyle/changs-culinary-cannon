using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Base;

namespace RC.Engine
{
    public interface RCGameManagerFactory
    {
        RCGameManager GetInstance();
    }
}