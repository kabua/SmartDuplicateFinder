using Ninject;
using System;
using System.Windows;

namespace SmartDuplicateFinder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            Current = null!;
        }

        public App()
        {
            Current = this;

            // try... catch doesn't work at this low level.
            //
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
	        base.OnStartup(e);

	        ConfigureContainer();
        }

        public static new App Current { get; private set; }

        public static string Name => Constants.AppName;

        public IKernel Container { get; private set; }

        private void ConfigureContainer() => Container = new StandardKernel();

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            string message = $"Unhanded Exception{ (object)(args.IsTerminating ? ", the application is terminating." : "")}";

            if (args.ExceptionObject is Exception exception)
            {
                message += Environment.NewLine + "Error: " + Environment.NewLine;

                while (true)
                {
                    message += exception.Message;
                    if (exception.InnerException == null)
                        break;

                    message += Environment.NewLine + "Inner: " + Environment.NewLine;
                    exception = exception.InnerException;
                }

                if (MainWindow != null)
                    MessageBox.Show(MainWindow, message, Name, MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(message, Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string msg = $"{ (object)message}.\nError Object Type:{ (object?)args.ExceptionObject.GetType().FullName}\nError Object:{args.ExceptionObject}";

                if (MainWindow != null)
                    MessageBox.Show(MainWindow, msg, Name, MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(msg, Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
