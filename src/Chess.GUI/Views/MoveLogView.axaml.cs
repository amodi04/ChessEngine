using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views
{
    /// <summary>
    /// Code behind for the view.
    /// </summary>
    public class MoveLogView : UserControl
    {
        public DataGrid DataGrid { get; }
        /// <summary>
        /// Initialise class with constructor.
        /// </summary>
        public MoveLogView()
        {
            InitializeComponent();
            DataGrid = this.Find<DataGrid>("MoveLogDataGrid");
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