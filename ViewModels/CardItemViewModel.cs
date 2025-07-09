using ReactiveUI;

namespace ASTEM_DB.ViewModels
{
    public class CardItemViewModel : ViewModelBase
    {
        private string _id = string.Empty;
        public string Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

    
    }
}