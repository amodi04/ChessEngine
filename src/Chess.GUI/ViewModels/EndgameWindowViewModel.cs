using ReactiveUI;

namespace Chess.GUI.ViewModels;

public class EndgameWindowViewModel : ViewModelBase
{
    private string _endgameStatus;

    public EndgameWindowViewModel()
    {
        _endgameStatus = "";
    }

    public string EndgameStatus
    {
        get => _endgameStatus;
        set => this.RaiseAndSetIfChanged(ref _endgameStatus, value);
    }
}