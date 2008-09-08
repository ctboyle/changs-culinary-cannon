using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using RC.Engine.Cameras;

namespace RC.Engine.Test
{
    class Wedge : RC.Engine.GraphicsManagement.RCSpatial
    {
        private Model _wedgeModel = null;

        public override void Load(ContentManager content)
        {
            _wedgeModel = content.Load<Model>(@"Content/enemy");
        }

        public override void Unload()
        {
            _wedgeModel = null;
        }

        public override void Draw(RC.Engine.Rendering.IRCRenderManager render)
        {
            render.RenderModel(
                _wedgeModel,
                delegate(GraphicsDevice g, IRCRenderManager r)
                {
                    r.SetWorld(WorldTrans);
                }
            );
        }

        protected override void UpdateWorldBound()
        {
           
        }
    }
}