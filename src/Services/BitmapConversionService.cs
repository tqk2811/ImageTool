using Emgu.CV;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageTool.Services
{
    public static class BitmapConversionService
    {
        public static BitmapSource ToBitmapSource(Mat mat)
        {
            if (mat == null || mat.IsEmpty)
                return null!;

            // Ensure the image is in Bgr8 or Bgra8 format
            // mat.Step is the number of bytes per row
            PixelFormat format = mat.NumberOfChannels == 3 ? PixelFormats.Bgr24 : PixelFormats.Bgra32;
            
            BitmapSource source = BitmapSource.Create(
                mat.Width,
                mat.Height,
                96,
                96,
                format,
                null,
                mat.DataPointer,
                mat.Step * mat.Height,
                mat.Step);

            source.Freeze();
            return source;
        }

        public static Mat ToMat(BitmapSource source)
        {
            if (source == null) return null!;

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * ((source.Format.BitsPerPixel + 7) / 8);

            Mat mat = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv8U, stride / width);
            source.CopyPixels(new Int32Rect(0, 0, width, height), mat.DataPointer, stride * height, stride);

            return mat;
        }
    }
}
