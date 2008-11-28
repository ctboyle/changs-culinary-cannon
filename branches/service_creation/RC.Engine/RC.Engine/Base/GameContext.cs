using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.StateManagement;
using RC.Engine.ContentManagement;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Base
{
    public interface RCGameContext
    {
        IRCContentRequester ContentRqst { get; }
        IRCGameStateManager StateMgr { get; }
        IRCGameStateStack StateStack { get; }
        IRCCameraManager CameraMgr { get; }
        IRCRenderManager RenderMgr { get; }
        IGraphicsDeviceService Graphics { get; }
    }
}