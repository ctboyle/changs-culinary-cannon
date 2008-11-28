using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Core.Infrastructure;
using Ninject.Core.Interception;
using Ninject.Core;

namespace RC.Engine.Ninject
{
    public class NinjectUpdatePluginAttribute : InterceptAttribute
    {
        public override IInterceptor CreateInterceptor(IRequest request)
        {
            return request.Kernel.Get<NinjectUpdatePluginInterceptor>();
        }
    }
}