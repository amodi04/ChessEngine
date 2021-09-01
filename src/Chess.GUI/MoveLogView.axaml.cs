using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chess.GUI
{
    /// <summary>
    /// Code behind for the view.
    /// </summary>
    public class MoveLogView : UserControl
    {
        /// <summary>
        /// Initialise class with constructor.
        /// </summary>
        public MoveLogView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load xaml object associated with class.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}