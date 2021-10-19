using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;
using Chess.GUI.Views;
using LibVLCSharp.Shared;

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
            Core.Initialize();
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        ///     Called when application has loaded internally.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Load the main window
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel()
                };

                desktop.Exit += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var vm = (MainWindowViewModel)desktop.MainWindow?.DataContext;
                if (vm != null)
                    vm.Dispose();
            }
        }

    }
}