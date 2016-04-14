using Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl;
using Ninject.Extensions.Interception;

namespace Com.Pinz.Client.RemoteServiceConsumer.Infrastructure
{
    public class ChannelFactoryInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            ServiceBase serviceBase = invocation.Request.Target as ServiceBase;
            serviceBase.OpenChannel();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                serviceBase.CloseChannel();
            }
        }
    }
}
