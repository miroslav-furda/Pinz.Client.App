using Ninject.Extensions.Interception;
using System;

namespace Com.Pinz.Client.RemoteServiceConsumer.Infrastructure
{
    class ExceptionInterceptor : IInterceptor
    {
        public ExceptionInterceptor()
        {
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception)
            {
                //exceptionHandlerService.HandleException(exception);
            }
        }
    }
}
