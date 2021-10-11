using ReactiveUI;

namespace Chess.GUI.ViewModels
{
    public class EndgameWindowViewModel : ViewModelBase
    {
        // Member fields
        private string _endgameStatus;

        public string EndgameStatus
        {
            get => _endgameStatus;
            
            // Set and raise event if value changed
            set => this.RaiseAndSetIfChanged(ref _endgameStatus, value);
        }

        // Constructor
        public EndgameWindowViewModel()
        {
            // Initialise string
            EndgameStatus = "";
        }
    }
}