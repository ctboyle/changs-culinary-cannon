using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.ContentManagement;

namespace RC.Engine.Utility
{
    public interface IRCLoadable
    {
        void Request(IRCContentRequester request);
        void Load(IRCContentLoader loader);
        void Unload(IRCContentUnloader unloader);
    }
}
