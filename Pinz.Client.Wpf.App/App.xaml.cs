using Com.Pinz.Client.Wpf.App.Insights;
using Common.Logging;
using System;
using System.Windows;

namespace Com.Pinz.Client.Wpf.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger<App>();
        private ApplicationInsightHelper applicationInsightHelper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            applicationInsightHelper = new ApplicationInsightHelper();
            ApplicationBootstrapper bootstrapper = new ApplicationBootstrapper();
            bootstrapper.Run();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is TimeoutException)
            {
                MessageBox.Show(Wpf.App.Properties.Resources.Error_Timeout_Content,
                    Wpf.App.Properties.Resources.Warning_MessageBox_Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Log.ErrorFormat("An unhandled exception just occurred:{0}", e.Exception, e.Exception.Message);
                applicationInsightHelper.TrackNonFatalExceptions(e.Exception);

                MessageBox.Show(Wpf.App.Properties.Resources.Error_Undefined_Content + e.Exception.Message,
                    Wpf.App.Properties.Resources.Error_MessageBox_Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            e.Handled = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            applicationInsightHelper.FlushData();
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            applicationInsightHelper.FlushData();

        }
    }
}
