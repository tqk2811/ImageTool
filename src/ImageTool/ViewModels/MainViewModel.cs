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
using System.Threading.Tasks;
using Emgu.CV.CvEnum;

namespace ImageTool.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private const string FiltersFile = "filters.json";
        private const string SettingsFile = "settings.json";
        private Mat? _originalMat;
        private Mat? _processedMat;
        private readonly OcrService _ocrService = new();

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

        [ObservableProperty]
        private string _ocrLanguage = "eng";

        [ObservableProperty]
        private bool _isOcrRunning = false;

        public ObservableCollection<ColorFilter> Filters { get; } = new();
        public ObservableCollection<OcrResult> OcrResults { get; } = new();

        public ObservableCollection<string> OcrLanguages { get; } = new();

        public IRelayCommand OpenImageCommand { get; }
        public IRelayCommand PasteImageCommand { get; }
        public IRelayCommand AddFilterCommand { get; }
        public IRelayCommand<ColorFilter> RemoveFilterCommand { get; }
        public IRelayCommand RunOcrCommand { get; }

        public MainViewModel()
        {
            OpenImageCommand = new RelayCommand(OpenImage);
            PasteImageCommand = new RelayCommand(PasteImage);
            AddFilterCommand = new RelayCommand(AddFilter);
            RemoveFilterCommand = new RelayCommand<ColorFilter>(RemoveFilter);
            RunOcrCommand = new AsyncRelayCommand(RunOcrAsync);

            LoadAvailableLanguages();
            LoadFilters();

            Filters.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (ColorFilter item in e.NewItems)
                        item.PropertyChanged += Filter_PropertyChanged;
                if (e.OldItems != null)
                    foreach (ColorFilter item in e.OldItems)
                        item.PropertyChanged -= Filter_PropertyChanged;
                SaveFilters();
            };

            foreach (var filter in Filters)
                filter.PropertyChanged += Filter_PropertyChanged;

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

        private void LoadAvailableLanguages()
        {
            try
            {
                OcrLanguages.Clear();
                string tessdataPath = _ocrService.TessdataPath;
                if (Directory.Exists(tessdataPath))
                {
                    var files = Directory.GetFiles(tessdataPath, "*.traineddata");
                    foreach (var file in files)
                    {
                        OcrLanguages.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }

                if (OcrLanguages.Count == 0)
                {
                    OcrLanguages.Add("eng"); // Fallback
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading OCR languages: {ex.Message}");
                if (OcrLanguages.Count == 0) OcrLanguages.Add("eng");
            }
        }

        partial void OnOcrLanguageChanged(string value)
        {
            SaveSettings();
            // Re-init engine với ngôn ngữ mới
            _ocrService.InitializeOcr(value);
        }

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

                        OcrLanguage = settings.OcrLanguage;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            // Khởi tạo engine sau khi load language
            _ocrService.InitializeOcr(OcrLanguage);
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
                    IsOcrClosingEnabled = IsOcrClosingEnabled,
                    OcrLanguage = OcrLanguage
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
                CvInvoke.CvtColor(_originalMat, hsv, ColorConversion.Bgr2Hsv);

                using Mat combinedMask = new Mat(hsv.Size, DepthType.Cv8U, 1);
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

                _processedMat?.Dispose();

                if (!hasActiveFilter)
                {
                    _processedMat = new Mat(_originalMat.Size, _originalMat.Depth, _originalMat.NumberOfChannels);
                    _processedMat.SetTo(new MCvScalar(0, 0, 0));
                }
                else
                {
                    _processedMat = new Mat();
                    _originalMat.CopyTo(_processedMat, combinedMask);
                }

                // Áp dụng Morphological Ops nếu bật
                _processedMat = ApplyMorphology(_processedMat);

                ProcessedImage = BitmapConversionService.ToBitmapSource(_processedMat);

                // Nếu OCR đang bật thì tự động chạy
                if (IsOcrEnabled)
                    _ = RunOcrAsync();
                else
                    OcrResults.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        /// <summary>Áp dụng Scale / Erosion / Dilation / Closing lên ảnh đã mask.</summary>
        private Mat ApplyMorphology(Mat input)
        {
            if (input == null || input.IsEmpty) return input;

            Mat current = input;

            // Scale: dùng Resize nếu cần (chỉ áp dụng để hiển thị; OCR sẽ tự scale)
            // Erosion
            if (IsOcrErosionEnabled && OcrErosionSize > 0)
            {
                Mat eroded = new Mat();
                int sz = OcrErosionSize;
                using var kernel = CvInvoke.GetStructuringElement(MorphShapes.Rectangle,
                    new System.Drawing.Size(2 * sz + 1, 2 * sz + 1),
                    new System.Drawing.Point(sz, sz));
                CvInvoke.Erode(current, eroded, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                if (current != input) current.Dispose();
                current = eroded;
            }

            // Dilation
            if (IsOcrDilationEnabled && OcrDilationSize > 0)
            {
                Mat dilated = new Mat();
                int sz = OcrDilationSize;
                using var kernel = CvInvoke.GetStructuringElement(MorphShapes.Rectangle,
                    new System.Drawing.Size(2 * sz + 1, 2 * sz + 1),
                    new System.Drawing.Point(sz, sz));
                CvInvoke.Dilate(current, dilated, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                if (current != input) current.Dispose();
                current = dilated;
            }

            // Closing = Dilation rồi Erosion
            if (IsOcrClosingEnabled && OcrClosingSize > 0)
            {
                Mat closed = new Mat();
                int sz = OcrClosingSize;
                using var kernel = CvInvoke.GetStructuringElement(MorphShapes.Rectangle,
                    new System.Drawing.Size(2 * sz + 1, 2 * sz + 1),
                    new System.Drawing.Point(sz, sz));
                CvInvoke.MorphologyEx(current, closed, MorphOp.Close, kernel,
                    new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                if (current != input) current.Dispose();
                current = closed;
            }

            return current;
        }

        private async Task RunOcrAsync()
        {
            if (_processedMat == null || _processedMat.IsEmpty || IsOcrRunning) return;

            IsOcrRunning = true;
            Mat matSnapshot = _processedMat.Clone(); // snapshot để chạy trên bg thread
            double scale = IsOcrScaleEnabled ? OcrScaleFactor : 1.0;

            try
            {
                var results = await Task.Run(() => _ocrService.PerformOcr(matSnapshot, scale));

                OcrResults.Clear();
                foreach (var r in results)
                    OcrResults.Add(r);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OCR] RunOcrAsync error: {ex.Message}");
            }
            finally
            {
                matSnapshot.Dispose();
                IsOcrRunning = false;
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
                Filters.Remove(filter);
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
                    _originalMat = CvInvoke.Imread(openFileDialog.FileName, Emgu.CV.CvEnum.ImreadModes.AnyColor);

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
                    System.Windows.MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }
    }
}
