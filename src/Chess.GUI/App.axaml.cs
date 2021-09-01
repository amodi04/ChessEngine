using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Chess.GUI
{
    /// <summary>
    /// Main application.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initialise the front end xaml.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when application has loaded internally.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            // If the application is a desktop application
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Create the main window
                desktop.MainWindow = new MainWindow();
            }

            // Call the base completed event
            base.OnFrameworkInitializationCompleted();
        }
    }
}