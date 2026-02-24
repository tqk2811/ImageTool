using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageTool.Models
{
    public partial class ColorFilter : ObservableObject
    {
        [ObservableProperty]
        private bool _isUsed = true;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _note = string.Empty;

        [ObservableProperty]
        private double _hMin = 0;

        [ObservableProperty]
        private double _hMax = 180;

        [ObservableProperty]
        private double _sMin = 0;

        [ObservableProperty]
        private double _sMax = 255;

        [ObservableProperty]
        private double _vMin = 0;

        [ObservableProperty]
        private double _vMax = 255;

        public ColorFilter() { }

        public ColorFilter(string name)
        {
            Name = name;
        }
    }
}
