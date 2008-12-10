using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace RC.Engine.ContentManagement.ModelReader
{
    public class RCModelContent
    {
        public RCModelContent(ModelContent content)
        {
            Content = content;
        }

        public ModelContent Content;
    }
}
