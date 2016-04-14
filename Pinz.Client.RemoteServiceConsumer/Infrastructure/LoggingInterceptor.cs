using Ninject.Extensions.Interception;
using System;
using System.Diagnostics;
using System.Linq;

namespace Com.Pinz.Client.RemoteServiceConsumer.Infrastructure
{
    internal class LoggingInterceptor : IInterceptor
    {
        private readonly static TraceSource Tracer = new TraceSource("Com.Pinz.Client.ServiceConsumer.Services");

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Request.Method.Name;

            try
            {
                var parameterNames = invocation.Request.Method.GetParameters().Select(p => p.Name).ToList();
                var parameterValues = invocation.Request.Arguments;

                var message = string.Format("Method {0} called with parameters ", methodName);
                for (int index = 0; index < parameterNames.Count; index++)
                {
                    var name = parameterNames[index];
                    var value = parameterValues[index];
                    message += string.Format("<{0}>:<{1}>,", name, value);
                }

                //log method called
                LogMessage(message);

                //run the intercepted method
                invocation.Proceed();

                //log method return value
                if (invocation.Request.Method.ReturnType != typeof(void))
                {
                    LogMessage(string.Format("Method {0} returned <{1}>", methodName, invocation.ReturnValue));
                }
            }
            catch (Exception ex)
            {
                var message = string.Format("Method {0} EXCEPTION occured: {1} ", methodName, ex);
                LogMessage(message);
                throw;
            }
        }

        private static void LogMessage(string message)
        {
            Tracer.TraceInformation("{0:HH:mm} {1}", DateTime.Now, message);
        }
    }
}
