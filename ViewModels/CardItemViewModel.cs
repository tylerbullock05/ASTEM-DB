using ReactiveUI;

namespace ASTEM_DB.ViewModels
{
    public class CardItemViewModel : ViewModelBase
    {
        private string _id = string.Empty;
        private Avalonia.Media.Imaging.Bitmap _image = null!;
        private string _glazeType = string.Empty;
        private double _colorL;
        private double _colorA;
        private double _colorB;
        private string _lab = string.Empty;
        private string _firingType = string.Empty;
        private string _soilType = string.Empty;
        private string _chemicalComposition = string.Empty;
        private string _surfaceCondition = string.Empty;
        private string _colorName = string.Empty;
        private string _imagePath = string.Empty;
        public string Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        public string ImagePath
        {
            get => _imagePath;
            set => this.RaiseAndSetIfChanged(ref _imagePath, value);
        }

        public Avalonia.Media.Imaging.Bitmap Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public string GlazeType
        {
            get => _glazeType;
            set
            {
                this.RaiseAndSetIfChanged(ref _glazeType, value);
            }
        }


        public double ColorL
        {
            get => _colorL;
            set => this.RaiseAndSetIfChanged(ref _colorL, value);
        }

        public double ColorA
        {
            get => _colorA;
            set => this.RaiseAndSetIfChanged(ref _colorA, value);
        }

        public double ColorB
        {
            get => _colorB;
            set => this.RaiseAndSetIfChanged(ref _colorB, value);
        }

        public string Lab
        {
            get => _lab;
            set => this.RaiseAndSetIfChanged(ref _lab, value);
        }

        public string FiringType
        {
            get => _firingType;
            set
            {
                this.RaiseAndSetIfChanged(ref _firingType, value);
            }
        }

        public string SoilType
        {
            get => _soilType;
            set
            {
                this.RaiseAndSetIfChanged(ref _soilType, value);
            }
        }

        public string ChemicalComposition
        {
            get => _chemicalComposition;
            set
            {
                this.RaiseAndSetIfChanged(ref _chemicalComposition, value);
            }
        }
        public string SurfaceCondition
        {
            get => _firingType;
            set
            {
                this.RaiseAndSetIfChanged(ref _surfaceCondition, value);
            }
        }
        public string ColorName
        {
            get => _colorName;
            set => this.RaiseAndSetIfChanged(ref _colorName, value);
        }
    }
}