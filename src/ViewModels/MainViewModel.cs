using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System;
using System.IO;
using Emgu.CV;
using ImageTool.Services;
using System.Windows;
using System.Collections.ObjectModel;
using ImageTool.Models;
using System.Text.Json;
using System.Linq;
using System.ComponentModel;
using Emgu.CV.Structure;
using System.Windows.Media;

namespace ImageTool.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private const string FiltersFile = "filters.json";
        private const string SettingsFile = "settings.json";
        private Mat? _originalMat;
        private Mat? _processedMat;

        [ObservableProperty]
        private BitmapSource? _originalImage;

        [ObservableProperty]
        private BitmapSource? _processedImage;

        [ObservableProperty]
        private bool _useCheckeredBackground = true;

        [ObservableProperty]
        private double _checkerSize = 20;

        [ObservableProperty]
        private Color _selectedBackgroundColor = Colors.Gray;

        [ObservableProperty]
        private bool _isSolidBackground = false;

        [ObservableProperty]
        private double _controlPanelWidth = 350;

        [ObservableProperty]
        private bool _isOcrEnabled = false;

        [ObservableProperty]
        private double _ocrScaleFactor = 1.0;

        [ObservableProperty]
        private int _ocrErosionSize = 0;

        [ObservableProperty]
        private int _ocrDilationSize = 0;

        [ObservableProperty]
        private int _ocrClosingSize = 0;

        [ObservableProperty]
        private bool _isOcrScaleEnabled = true;

        [ObservableProperty]
        private bool _isOcrErosionEnabled = true;

        [ObservableProperty]
        private bool _isOcrDilationEnabled = true;

        [ObservableProperty]
        private bool _isOcrClosingEnabled = true;

        public ObservableCollection<ColorFilter> Filters { get; } = new();

        public IRelayCommand OpenImageCommand { get; }
        public IRelayCommand PasteImageCommand { get; }
        public IRelayCommand AddFilterCommand { get; }
        public IRelayCommand<ColorFilter> RemoveFilterCommand { get; }

        public MainViewModel()
        {
            OpenImageCommand = new RelayCommand(OpenImage);
            PasteImageCommand = new RelayCommand(PasteImage);
            AddFilterCommand = new RelayCommand(AddFilter);
            RemoveFilterCommand = new RelayCommand<ColorFilter>(RemoveFilter);

            LoadFilters();
            
            // Subscribe to collection changes to handle persistence
            Filters.CollectionChanged += (s, e) => 
            {
                if (e.NewItems != null)
                {
                    foreach (ColorFilter item in e.NewItems)
                        item.PropertyChanged += Filter_PropertyChanged;
                }
                if (e.OldItems != null)
                {
                    foreach (ColorFilter item in e.OldItems)
                        item.PropertyChanged -= Filter_PropertyChanged;
                }
                SaveFilters();
            };

            foreach (var filter in Filters)
            {
                filter.PropertyChanged += Filter_PropertyChanged;
            }

            LoadSettings();
        }

        private void Filter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SaveFilters();
            ApplyFilters();
        }

        partial void OnControlPanelWidthChanged(double value) => SaveSettings();
        partial void OnCheckerSizeChanged(double value) => SaveSettings();
        partial void OnSelectedBackgroundColorChanged(Color value) => SaveSettings();
        partial void OnIsSolidBackgroundChanged(bool value) => SaveSettings();
        
        partial void OnIsOcrEnabledChanged(bool value) { SaveSettings(); ApplyFilters(); }
        partial void OnOcrScaleFactorChanged(double value) { SaveSettings(); ApplyFilters(); }
        partial void OnOcrErosionSizeChanged(int value) { SaveSettings(); ApplyFilters(); }
        partial void OnOcrDilationSizeChanged(int value) { SaveSettings(); ApplyFilters(); }
        partial void OnOcrClosingSizeChanged(int value) { SaveSettings(); ApplyFilters(); }
        
        partial void OnIsOcrScaleEnabledChanged(bool value) { SaveSettings(); ApplyFilters(); }
        partial void OnIsOcrErosionEnabledChanged(bool value) { SaveSettings(); ApplyFilters(); }
        partial void OnIsOcrDilationEnabledChanged(bool value) { SaveSettings(); ApplyFilters(); }
        partial void OnIsOcrClosingEnabledChanged(bool value) { SaveSettings(); ApplyFilters(); }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                    {
                        ControlPanelWidth = settings.ControlPanelWidth;
                        CheckerSize = settings.CheckerSize;
                        IsSolidBackground = settings.IsSolidBackground;
                        SelectedBackgroundColor = Color.FromArgb(
                            settings.SelectedBackgroundColor[0],
                            settings.SelectedBackgroundColor[1],
                            settings.SelectedBackgroundColor[2],
                            settings.SelectedBackgroundColor[3]);
                        
                        IsOcrEnabled = settings.IsOcrEnabled;
                        OcrScaleFactor = settings.OcrScaleFactor;
                        OcrErosionSize = settings.OcrErosionSize;
                        OcrDilationSize = settings.OcrDilationSize;
                        OcrClosingSize = settings.OcrClosingSize;

                        IsOcrScaleEnabled = settings.IsOcrScaleEnabled;
                        IsOcrErosionEnabled = settings.IsOcrErosionEnabled;
                        IsOcrDilationEnabled = settings.IsOcrDilationEnabled;
                        IsOcrClosingEnabled = settings.IsOcrClosingEnabled;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new AppSettings
                {
                    ControlPanelWidth = ControlPanelWidth,
                    CheckerSize = CheckerSize,
                    IsSolidBackground = IsSolidBackground,
                    SelectedBackgroundColor = new byte[] { 
                        SelectedBackgroundColor.A, 
                        SelectedBackgroundColor.R, 
                        SelectedBackgroundColor.G, 
                        SelectedBackgroundColor.B 
                    },
                    IsOcrEnabled = IsOcrEnabled,
                    OcrScaleFactor = OcrScaleFactor,
                    OcrErosionSize = OcrErosionSize,
                    OcrDilationSize = OcrDilationSize,
                    OcrClosingSize = OcrClosingSize,
                    IsOcrScaleEnabled = IsOcrScaleEnabled,
                    IsOcrErosionEnabled = IsOcrErosionEnabled,
                    IsOcrDilationEnabled = IsOcrDilationEnabled,
                    IsOcrClosingEnabled = IsOcrClosingEnabled
                };
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            if (_originalMat == null || _originalMat.IsEmpty) return;

            try
            {
                using Mat hsv = new Mat();
                CvInvoke.CvtColor(_originalMat, hsv, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

                using Mat combinedMask = new Mat(hsv.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
                combinedMask.SetTo(new MCvScalar(0));

                bool hasActiveFilter = false;
                foreach (var filter in Filters)
                {
                    if (!filter.IsUsed) continue;
                    hasActiveFilter = true;

                    using Mat mask = new Mat();
                    CvInvoke.InRange(hsv, 
                        new ScalarArray(new MCvScalar(filter.HMin, filter.SMin, filter.VMin)),
                        new ScalarArray(new MCvScalar(filter.HMax, filter.SMax, filter.VMax)),
                        mask);

                    CvInvoke.BitwiseOr(combinedMask, mask, combinedMask);
                }

                if (!hasActiveFilter)
                {
                    // If no filter is active, we could show nothing or a black mask. 
                    // Let's show the original but with transparency or just black for now.
                    _processedMat?.Dispose();
                    _processedMat = new Mat(_originalMat.Size, _originalMat.Depth, _originalMat.NumberOfChannels);
                    _processedMat.SetTo(new MCvScalar(0, 0, 0));
                }
                else
                {
                    _processedMat?.Dispose();
                    _processedMat = new Mat();
                    _originalMat.CopyTo(_processedMat, combinedMask);
                }

                ProcessedImage = BitmapConversionService.ToBitmapSource(_processedMat);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void LoadFilters()
        {
            try
            {
                if (File.Exists(FiltersFile))
                {
                    string json = File.ReadAllText(FiltersFile);
                    var loadedFilters = JsonSerializer.Deserialize<List<ColorFilter>>(json);
                    if (loadedFilters != null)
                    {
                        Filters.Clear();
                        foreach (var filter in loadedFilters)
                            Filters.Add(filter);
                    }
                }
                else
                {
                    // Default filters if file doesn't exist
                    Filters.Add(new ColorFilter("Trắng") { HMin = 0, HMax = 180, SMin = 0, SMax = 30, VMin = 200, VMax = 255 });
                    Filters.Add(new ColorFilter("Cyan") { HMin = 80, HMax = 100, SMin = 100, SMax = 255, VMin = 100, VMax = 255 });
                    SaveFilters();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading filters: {ex.Message}");
            }
        }

        private void SaveFilters()
        {
            try
            {
                string json = JsonSerializer.Serialize(Filters.ToList(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FiltersFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving filters: {ex.Message}");
            }
        }

        private void AddFilter()
        {
            Filters.Add(new ColorFilter("New Filter"));
        }

        private void RemoveFilter(ColorFilter? filter)
        {
            if (filter != null)
            {
                Filters.Remove(filter);
            }
        }

        private void PasteImage()
        {
            if (Clipboard.ContainsImage())
            {
                try
                {
                    BitmapSource source = Clipboard.GetImage();
                    _originalMat = BitmapConversionService.ToMat(source);
                    
                    if (_originalMat != null && !_originalMat.IsEmpty)
                    {
                        _processedMat = _originalMat.Clone();
                        OriginalImage = BitmapConversionService.ToBitmapSource(_originalMat);
                        ProcessedImage = BitmapConversionService.ToBitmapSource(_processedMat);
                        ApplyFilters();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error pasting image: {ex.Message}");
                }
            }
        }

        private void OpenImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load image using Emgu.CV
                    _originalMat = CvInvoke.Imread(openFileDialog.FileName, Emgu.CV.CvEnum.ImreadModes.AnyColor);
                    
                    if (_originalMat != null && !_originalMat.IsEmpty)
                    {
                        // Convert BGR (Emgu.CV default) to BGRA for transparency support later
                        _processedMat = _originalMat.Clone();
                        
                        // Update UI properties
                        OriginalImage = BitmapConversionService.ToBitmapSource(_originalMat);
                        ProcessedImage = BitmapConversionService.ToBitmapSource(_processedMat);
                        ApplyFilters();
                    }
                }
                catch (Exception ex)
                {
                    // Handle error (should ideally use a DialogService)
                    System.Windows.MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }
    }
}
