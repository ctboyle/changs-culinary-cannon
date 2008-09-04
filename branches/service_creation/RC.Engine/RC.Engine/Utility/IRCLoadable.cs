using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Utility
{
    public interface IRCLoadable
    {
        void Load();
        void Unload();
    }
}
