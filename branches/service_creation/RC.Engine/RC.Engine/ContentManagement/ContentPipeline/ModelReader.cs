using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace RC.Engine.ContentManagement.ModelReader
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class RCModelReader : ContentTypeReader<RCModelContent>
    {
        protected override RCModelContent Read(ContentReader input, RCModelContent existingInstance)
        {
            ModelContent content = input.ReadRawObject<ModelContent>();
            //ModelContent content = input.ReadObject<ModelContent>();

            return new RCModelContent(content);
        }
    }
}
