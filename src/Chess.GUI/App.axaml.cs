using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Chess.GUI.Views;

namespace Chess.GUI
{
    /// <summary>
    ///     Main application loop.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        ///     Initialises GUI components
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        ///     Called when application has loaded internally.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                // Load the main window
                desktop.MainWindow = new MainWindow();
            
            base.OnFrameworkInitializationCompleted();
        }
    }
}