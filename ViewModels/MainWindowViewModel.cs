using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;

namespace ASTEM_DB.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Initialize fields at declaration to prevent CS8618 warnings
        private ObservableCollection<CardItemViewModel> _cardItems = new ObservableCollection<CardItemViewModel>();
        public ObservableCollection<CardItemViewModel> CardItems
        {
            get => _cardItems;
            set => this.RaiseAndSetIfChanged(ref _cardItems, value);
        }

        private bool _sortByNameChecked;
        public bool SortByNameChecked
        {
            get => _sortByNameChecked;
            set => this.RaiseAndSetIfChanged(ref _sortByNameChecked, value);
        }

        // Properties for the color picker (simplified for layout purposes)
        private string _hexColor = string.Empty; // Initialize to empty string
        public string HexColor
        {
            get => _hexColor;
            set => this.RaiseAndSetIfChanged(ref _hexColor, value);
        }

        private int _red;
        public int Red
        {
            get => _red;
            set => this.RaiseAndSetIfChanged(ref _red, value);
        }

        private int _green;
        public int Green
        {
            get => _green;
            set => this.RaiseAndSetIfChanged(ref _green, value);
        }

        private int _blue;
        public int Blue
        {
            get => _blue;
            set => this.RaiseAndSetIfChanged(ref _blue, value);
        }

        // public ReactiveCommand<Unit, Unit> SortCommand { get; }

        public MainWindowViewModel()
        {
            // _cardItems is initialized above, so no need to re-initialize here.
            for (int i = 1; i <= 20; i++)             {
                CardItems.Add(new CardItemViewModel { Id = $"ID {i}" });
            }

            // Initialize color picker values
            HexColor = "#9B59B6";
            Red = 155;
            Green = 89;
            Blue = 182;
        }
        public string SortCommand()
        {
            Debug.WriteLine("SortCommand executed");
            return "Papple";
        }
        
    }
}