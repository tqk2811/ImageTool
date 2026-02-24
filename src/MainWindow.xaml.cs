using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageTool.ViewModels;

namespace ImageTool
{
    public partial class MainWindow : Window
    {
        private Point _lastOriginalMousePosition;
        private Point _lastProcessedMousePosition;
        private bool _isOriginalPanning;
        private bool _isProcessedPanning;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void OriginalPreview_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control || true) // Always allow zoom for simplicity now
            {
                double zoomStep = e.Delta > 0 ? 1.2 : 1 / 1.2;
                double newScale = OriginalScaleTransform.ScaleX * zoomStep;

                // Limit scale
                if (newScale >= 0.1 && newScale <= 20)
                {
                    OriginalScaleTransform.ScaleX = newScale;
                    OriginalScaleTransform.ScaleY = newScale;
                }
                
                e.Handled = true;
            }
        }

        private void OriginalPreview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isOriginalPanning = true;
                _lastOriginalMousePosition = e.GetPosition(OriginalScrollViewer);
                OriginalScrollViewer.CaptureMouse();
                OriginalScrollViewer.Cursor = Cursors.Hand;
            }
        }

        private void OriginalPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isOriginalPanning)
            {
                Point currentPosition = e.GetPosition(OriginalScrollViewer);
                double deltaX = currentPosition.X - _lastOriginalMousePosition.X;
                double deltaY = currentPosition.Y - _lastOriginalMousePosition.Y;

                OriginalScrollViewer.ScrollToHorizontalOffset(OriginalScrollViewer.HorizontalOffset - deltaX);
                OriginalScrollViewer.ScrollToVerticalOffset(OriginalScrollViewer.VerticalOffset - deltaY);

                _lastOriginalMousePosition = currentPosition;
            }
        }

        private void OriginalPreview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isOriginalPanning)
            {
                _isOriginalPanning = false;
                OriginalScrollViewer.ReleaseMouseCapture();
                OriginalScrollViewer.Cursor = Cursors.Arrow;
            }
        }

        private void ProcessedPreview_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomStep = e.Delta > 0 ? 1.2 : 1 / 1.2;
            double newScale = ProcessedScaleTransform.ScaleX * zoomStep;

            if (newScale >= 0.1 && newScale <= 20)
            {
                ProcessedScaleTransform.ScaleX = newScale;
                ProcessedScaleTransform.ScaleY = newScale;
            }
            e.Handled = true;
        }

        private void ProcessedPreview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isProcessedPanning = true;
                _lastProcessedMousePosition = e.GetPosition(ProcessedScrollViewer);
                ProcessedScrollViewer.CaptureMouse();
                ProcessedScrollViewer.Cursor = Cursors.Hand;
            }
        }

        private void ProcessedPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isProcessedPanning)
            {
                Point currentPosition = e.GetPosition(ProcessedScrollViewer);
                double deltaX = currentPosition.X - _lastProcessedMousePosition.X;
                double deltaY = currentPosition.Y - _lastProcessedMousePosition.Y;

                ProcessedScrollViewer.ScrollToHorizontalOffset(ProcessedScrollViewer.HorizontalOffset - deltaX);
                ProcessedScrollViewer.ScrollToVerticalOffset(ProcessedScrollViewer.VerticalOffset - deltaY);

                _lastProcessedMousePosition = currentPosition;
            }
        }

        private void ProcessedPreview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isProcessedPanning)
            {
                _isProcessedPanning = false;
                ProcessedScrollViewer.ReleaseMouseCapture();
                ProcessedScrollViewer.Cursor = Cursors.Arrow;
            }
        }
    }
}
