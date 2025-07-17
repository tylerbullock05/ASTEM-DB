using ASTEM_DB.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
// using System.Diagnostics;

namespace ASTEM_DB.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DatabaseService _db = new();
        private ObservableCollection<CardItemViewModel> _cardItems = new ObservableCollection<CardItemViewModel>();
        public ObservableCollection<CardItemViewModel> CardItems
        {
            get => _cardItems;
            set => this.RaiseAndSetIfChanged(ref _cardItems, value);
        }

        public ObservableCollection<string> GlazeTypes { get; } = new();
        public ObservableCollection<string> SurfaceConditions { get; } = new();

        private string? _selectedGlazeType;
        public string? SelectedGlazeType
        {
            get => _selectedGlazeType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedGlazeType, value);
                FilterCardItems();
            }
        }

        private string? _selectedSurfaceCondition;
        public string? SelectedSurfaceCondition
        {
            get => _selectedSurfaceCondition;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedSurfaceCondition, value);
                FilterCardItems();
            }
        }

        public ObservableCollection<string> FiringTypes { get; } = new();

        private string? _selectedFiringType;
        public string? SelectedFiringType
        {
            get => _selectedFiringType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFiringType, value);
                FilterCardItems();
            }
        }

        private CardItemViewModel? _selectedCard;
        public CardItemViewModel? SelectedCard
        {
            get => _selectedCard;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCard, value);
                IsSidebarVisible = value != null;
            }
        }

        private bool _sortByNameChecked;
        public bool SortByNameChecked
        {
            get => _sortByNameChecked;
            set => this.RaiseAndSetIfChanged(ref _sortByNameChecked, value);
        }

        private bool _isSidebarVisible;
        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set => this.RaiseAndSetIfChanged(ref _isSidebarVisible, value);
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

        private int _red;
        public int Red
        {
            get => _red;
            set
            {
                int clamped = Math.Clamp(value, 0, 255);
                this.RaiseAndSetIfChanged(ref _red, clamped);
                UpdateHexColor();
                labConversion();
            }
        }

        private int _green;
        public int Green
        {
            get => _green;
            set
            {
                int clamped = Math.Clamp(value, 0, 255);
                this.RaiseAndSetIfChanged(ref _green, clamped);
                UpdateHexColor();
                labConversion();
            }
        }

        private int _blue;
        public int Blue
        {
            get => _blue;
            set
            {
                int clamped = Math.Clamp(value, 0, 255);
                this.RaiseAndSetIfChanged(ref _blue, clamped);
                UpdateHexColor();
                labConversion();
            }
        }

        private double _lightness;
        public double Lightness
        {
            get => _lightness;
            set
            {
                this.RaiseAndSetIfChanged(ref _lightness, value);
                FilterCardItems();
            }
        }
        private double _redGreen;
        public double RedGreen
        {
            get => _redGreen;
            set
            {
                this.RaiseAndSetIfChanged(ref _redGreen, value);
                FilterCardItems();
            }
        }
        private double _blueYellow;
        public double BlueYellow
        {
            get => _blueYellow;
            set
            {
                this.RaiseAndSetIfChanged(ref _blueYellow, value);
                FilterCardItems();
            }
        }

        public MainWindowViewModel()
        {
            LoadData();

            HexColor = "#9B59B6";
            Red = 185;
            Green = 145;
            Blue = 117;
            labConversion();
        }
        
        private async void FilterCardItems()
        {
            var allItems = await _db.GetFilteredCardItemsAsync(SelectedGlazeType, SelectedSurfaceCondition);

            var selectedLab = new Lab { L = Lightness, A = RedGreen, B = BlueYellow };
            double threshold = 10.0;


            var filtered = allItems.Where(item =>
            {
                var lab = new Lab { L = item.ColorL, A = item.ColorA, B = item.ColorB };
                double deltaE = selectedLab.Compare(lab, new Cie1976Comparison());
                return deltaE <= threshold;
            });

            CardItems.Clear();
            foreach (var item in filtered)
                CardItems.Add(item);
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

        public ObservableCollection<string> ColorPalettes { get; } = new();
        private string? _selectedColorPalette;
        public string? SelectedColorPalette
        {
            get => _selectedColorPalette;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedColorPalette, value);
                FilterCardItems();
            }
        }

        private async void LoadData()
        {
            var db = new DatabaseService();

            // Load Glaze Types
            GlazeTypes.Clear();
            GlazeTypes.Add("All");

            var glazeTypes = await db.GetGlazeTypesAsync();
            foreach (var type in glazeTypes)
                GlazeTypes.Add(type);

            // Load Surface Conditions
            SurfaceConditions.Clear();
            SurfaceConditions.Add("All");

            var surfaceConditions = await db.GetSurfaceCondition();
            foreach (var sc in surfaceConditions)
                SurfaceConditions.Add(sc);

            FiringTypes.Clear();
            FiringTypes.Add("All");

            var firingTypes = await db.GetFiringType();
            foreach (var ft in firingTypes)
                FiringTypes.Add(ft);

            // Set ColorPalettes
            ColorPalettes.Clear();
            ColorPalettes.Add("Red");
            ColorPalettes.Add("Orange");
            ColorPalettes.Add("Yellow");
            ColorPalettes.Add("Green");
            ColorPalettes.Add("Blue");
            ColorPalettes.Add("Purple");

            // Set default filters
            SelectedGlazeType = "All";
            SelectedSurfaceCondition = "All";
            SelectedFiringType = "All";
            SelectedColorPalette = "Red";
        }

        private void labConversion()
        {
            var rgb = new Rgb { R = Red, G = Green, B = Blue };
            var lab = rgb.To<Lab>();
            Lightness = lab.L;
            RedGreen = lab.A;
            BlueYellow = lab.B;
        }

    }
}
