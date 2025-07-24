using ASTEM_DB.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using Avalonia.Media;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// using System.Diagnostics;
// using Microsoft.VisualBasic.FileIO;

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

        private CancellationTokenSource? _searchCts;

        public MainWindowViewModel()
        {
            LoadData();
            Red = 185;
            Green = 145;
            Blue = 117;
            labConversion();
        }

        public async void SearchCommand()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            try
            {
                await FilterCardItemsAsync(_searchCts.Token);
            }
            catch (OperationCanceledException)
            {
                // Search was canceled, do nothing
            }
        }

        private bool _isFilterEmpty;
        public bool IsFilterEmpty
        {
            get => _isFilterEmpty;
            set => this.RaiseAndSetIfChanged(ref _isFilterEmpty, value);
        }
        private bool _filterByString;
        public bool FilterByString
        {
            get => _filterByString;
            set => this.RaiseAndSetIfChanged(ref _filterByString, value);
        }
        private async void FilterCardItems()
        {
            await FilterCardItemsAsync(CancellationToken.None);
        }

        private async Task FilterCardItemsAsync(CancellationToken cancellationToken)
        {
            var allItems = await _db.GetFilteredCardItemMetadataAsync(SelectedGlazeType, SelectedSurfaceCondition, SelectedFiringType);

            var selectedLab = new Lab { L = Lightness, A = RedGreen, B = BlueYellow };
            double threshold = 25.0;

            foreach (var item in allItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var lab = new Lab { L = item.ColorL, A = item.ColorA, B = item.ColorB };
                item.ColorName = GetColorName(lab);
            }

            var filtered = allItems.Where(item =>
            {
                if (FilterByString)
                {
                    return item.ColorName == SelectedColorPalette;
                }
                else if (!FilterByColor)
                {
                    return true;
                }
                else
                {
                    var lab = new Lab { L = item.ColorL, A = item.ColorA, B = item.ColorB };
                    double deltaE = selectedLab.Compare(lab, new Cie1976Comparison());
                    return deltaE <= threshold;
                }
            }).ToList();

            CardItems.Clear();
            IsFilterEmpty = !filtered.Any();

            foreach (var item in filtered)
            {
                cancellationToken.ThrowIfCancellationRequested();

                item.Image = await _db.GetImageByIdAsync(item.Id);
                CardItems.Add(item);
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
            // GlazeTypes.Add("Glossy");
            // GlazeTypes.Add("Matte");

            // Load Surface Conditions
            SurfaceConditions.Clear();
            SurfaceConditions.Add("All");

            var surfaceConditions = await db.GetSurfaceCondition();
            foreach (var sc in surfaceConditions)
                SurfaceConditions.Add(sc);
            // SurfaceConditions.Add("Smooth");
            // SurfaceConditions.Add("Rough");

            FiringTypes.Clear();
            FiringTypes.Add("All");

            var firingTypes = await db.GetFiringType();
            foreach (var ft in firingTypes)
                FiringTypes.Add(ft);
            // FiringTypes.Add("")

            // Set ColorPalettes
            ColorPalettes.Clear();
            ColorPalettes.Add("Black");
            ColorPalettes.Add("White");
            ColorPalettes.Add("Red");
            ColorPalettes.Add("Green");
            ColorPalettes.Add("Yellow");
            ColorPalettes.Add("Blue");
            ColorPalettes.Add("Cyan");
            ColorPalettes.Add("Magenta");
            ColorPalettes.Add("Pink");
            ColorPalettes.Add("Gray");
            ColorPalettes.Add("Orange");
            ColorPalettes.Add("Brown");

            // Set default filters
            SelectedGlazeType = "All";
            SelectedSurfaceCondition = "All";
            SelectedFiringType = "All";
            SelectedColorPalette = "Blue";
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

        private static readonly Dictionary<string, Lab> LABColors = new()
        {
            { "Black", new Rgb { R = 0, G = 0, B = 0 }.To<Lab>() },
            { "White", new Rgb { R = 255, G = 255, B = 255 }.To<Lab>() },
            { "Red", new Rgb { R = 255, G = 0, B = 0 }.To<Lab>() },
            { "Green", new Rgb { R = 0, G = 255, B = 0 }.To<Lab>() },
            { "Blue", new Rgb { R = 0, G = 0, B = 255 }.To<Lab>() },
            { "Yellow", new Rgb { R = 255, G = 255, B = 0 }.To<Lab>() },
            { "Cyan", new Rgb { R = 0, G = 255, B = 255 }.To<Lab>() },
            { "Magenta", new Rgb { R = 255, G = 0, B = 255 }.To<Lab>() },
            { "Gray", new Rgb { R = 137, G = 137, B = 137 }.To<Lab>() },
            { "Orange", new Rgb { R = 255, G = 165, B = 0 }.To<Lab>() },
            { "Brown", new Rgb { R = 137, G = 81, B = 41 }.To<Lab>() },
            { "Pink", new Rgb { R = 255, G = 192, B = 203 }.To<Lab>() }
        };
        public static string GetColorName(Lab inputColor)
        {
            string colorName = "Unknown";
            double minDistance = double.MaxValue;

            foreach (var (name, color) in LABColors)
            {
                double distance = Math.Pow(inputColor.L - color.L, 2) +
                                  Math.Pow(inputColor.A - color.A, 2) +
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
