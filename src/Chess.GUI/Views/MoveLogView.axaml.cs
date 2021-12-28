using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views;

/// <summary>
///     Responsible passing data between engine and move log UI.
/// </summary>
public class MoveLogView : UserControl
{
    public MoveLogView()
    {
        InitializeComponent();
        DataGrid = this.Find<DataGrid>("MoveLogDataGrid");
    }

    public DataGrid DataGrid { get; }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}