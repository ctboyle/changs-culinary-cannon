using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Engine.Rendering
{
    class RenderConstants
    {
        delegate object GetValue();

        enum Constant
        {
            World,
            View,
            Projection
        }

        //private Dictionary<Constant, GetValue> _constants;


    }
}
