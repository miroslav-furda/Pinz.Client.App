using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl;
using Ninject;
using Ninject.Extensions.Interception;

namespace Com.Pinz.Client.RemoteServiceConsumer.Infrastructure
{
    public class ChannelFactoryInterceptor : IInterceptor
    {
        private IServiceRunningIndicator indicator;

        [Inject]
        public ChannelFactoryInterceptor(IServiceRunningIndicator indicator)
        {
            this.indicator = indicator;
        }

        public void Intercept(IInvocation invocation)
        {
            indicator.IsServiceRunning = true;
            ServiceBase serviceBase = invocation.Request.Target as ServiceBase;
            serviceBase.OpenChannel();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;
            }
        }
    }
}
