using ASTEM_DB.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using Avalonia.Media;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

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
            }
        }

        private string? _selectedSurfaceCondition;
        public string? SelectedSurfaceCondition
        {
            get => _selectedSurfaceCondition;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedSurfaceCondition, value);
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

        private int _red;
        public int Red
        {
            get => _red;
            set
            {
                var clamped = Math.Clamp(value, 0, 255);
                if (_red == clamped) return;

                _red = clamped;
                this.RaisePropertyChanged(nameof(Red));
                UpdateSelectedColor();
                labConversion();
            }
        }

        private int _green;
        public int Green
        {
            get => _green;
            set
            {
                var clamped = Math.Clamp(value, 0, 255);
                if (_green == clamped) return;

                _green = clamped;
                this.RaisePropertyChanged(nameof(Green));
                UpdateSelectedColor();
                labConversion();
            }
        }
        private int _blue;
        public int Blue
        {
            get => _blue;
            set
            {
                var clamped = Math.Clamp(value, 0, 255);
                if (_blue == clamped) return;

                _blue = clamped;
                this.RaisePropertyChanged(nameof(Blue));
                UpdateSelectedColor();
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
            }
        }
        private double _redGreen;
        public double RedGreen
        {
            get => _redGreen;
            set
            {
                this.RaiseAndSetIfChanged(ref _redGreen, value);
            }
        }
        private double _blueYellow;
        public double BlueYellow
        {
            get => _blueYellow;
            set
            {
                this.RaiseAndSetIfChanged(ref _blueYellow, value);
            }
        }

        public MainWindowViewModel()
        {
            LoadData();
            Red = 185;
            Green = 145;
            Blue = 117;
            labConversion();
        }

        public void SearchCommand()
        {
            FilterCardItems();
        }

        private async void FilterCardItems()
        {
            var allItems = await _db.GetFilteredCardItemsAsync(SelectedGlazeType, SelectedSurfaceCondition, SelectedFiringType);

            var selectedLab = new Lab { L = Lightness, A = RedGreen, B = BlueYellow };
            double threshold = 10.0;
            var filtered = allItems.Where(item =>
            {
                if (!FilterByColor)
                    return true;

                var lab = new Lab { L = item.ColorL, A = item.ColorA, B = item.ColorB };
                double deltaE = selectedLab.Compare(lab, new Cie1976Comparison());
                return deltaE <= threshold;
            });

            CardItems.Clear();
            foreach (var item in filtered)
            {
                CardItems.Add(item);
                var lab = new Lab { L = item.ColorL, A = item.ColorA, B = item.ColorB };
                var rgb = lab.To<Rgb>();
                item.ColorName = GetColorName(Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B));
            }
        }

        public ObservableCollection<string> ColorPalettes { get; } = new();
        private string? _selectedColorPalette;
        public string? SelectedColorPalette
        {
            get => _selectedColorPalette;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedColorPalette, value);
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

        private bool _filterByColor = true;
        public bool FilterByColor
        {
            get => _filterByColor;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterByColor, value);
            }
        }

        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (value == _selectedColor) return;
                _selectedColor = value;
                this.RaisePropertyChanged(nameof(SelectedColor));

                _red = value.R;
                _green = value.G;
                _blue = value.B;
                this.RaisePropertyChanged(nameof(Red));
                this.RaisePropertyChanged(nameof(Green));
                this.RaisePropertyChanged(nameof(Blue));
                labConversion();
            }
        }
        private void UpdateSelectedColor()
        {
            var newColor = Color.FromRgb((byte)Red, (byte)Green, (byte)Blue);
            if (_selectedColor == newColor) return;

            _selectedColor = newColor;
            this.RaisePropertyChanged(nameof(SelectedColor));
        }

        private static readonly Dictionary<string, Color> BasicColors = new()
        {
            { "Black", Color.FromRgb(0, 0, 0) },
            { "White", Color.FromRgb(255, 255, 255) },
            { "Red", Color.FromRgb(255, 0, 0) },
            { "Green", Color.FromRgb(0, 255, 0) },
            { "Blue", Color.FromRgb(0, 0, 255) },
            { "Yellow", Color.FromRgb(255, 255, 0) },
            { "Cyan", Color.FromRgb(0, 255, 255) },
            { "Magenta", Color.FromRgb(255, 0, 255) },
            { "Gray", Color.FromRgb(137, 137, 137) },
            { "Orange", Color.FromRgb(255, 165, 0) },
            { "Brown", Color.FromRgb(137, 81, 41) },
            { "Pink", Color.FromRgb(255, 192, 203) }
        };

        public static string GetColorName(Color inputColor)
        {
            string colorName = "Unknown";
            double minDistance = double.MaxValue;

            foreach (var (name, color) in BasicColors)
            {
                double distance = Math.Pow(inputColor.R - color.R, 2) +
                                  Math.Pow(inputColor.G - color.G, 2) +
                                  Math.Pow(inputColor.B - color.B, 2);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    colorName = name;
                }
            }
            return colorName;
        }
    }
}
