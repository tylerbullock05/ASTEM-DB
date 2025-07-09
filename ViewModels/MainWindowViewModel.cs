using ReactiveUI;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ASTEM_DB.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
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

        private string _hexColor = string.Empty; 
        public string HexColor
        {
            get => _hexColor;
            set
            {
                this.RaiseAndSetIfChanged(ref _hexColor, value);
                ParseHexToRGB(value);
            }
        }

        private void ParseHexToRGB(string hex)
        {
            if (!string.IsNullOrWhiteSpace(hex) && hex.StartsWith("#") && hex.Length == 7)
            {
                if (int.TryParse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out int r) &&
                    int.TryParse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out int g) &&
                    int.TryParse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out int b))
                {
                    _red = r;
                    _green = g;
                    _blue = b;

                    this.RaisePropertyChanged(nameof(Red));
                    this.RaisePropertyChanged(nameof(Green));
                    this.RaisePropertyChanged(nameof(Blue));
                }
            }
        }

        private void UpdateHexColor()
        {
            HexColor = $"#{Red:X2}{Green:X2}{Blue:X2}";
        }

        private int _red;
        public int Red
        {
            get => _red;
            set
            {
                this.RaiseAndSetIfChanged(ref _red, value);
                UpdateHexColor();
            }

        }

        private int _green;
        public int Green
        {
            get => _green;
            set
            {
                this.RaiseAndSetIfChanged(ref _green, value);
                UpdateHexColor();
            }
        }

        private int _blue;
        public int Blue
        {
            get => _blue;
            set
            {
                this.RaiseAndSetIfChanged(ref _blue, value); UpdateHexColor();
            }
        }

        public MainWindowViewModel()
        {
            for (int i = 1; i <= 20; i++)
            {
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