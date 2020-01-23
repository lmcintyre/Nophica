using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using SaintCoinach.Imaging;
using ImageConverter = SaintCoinach.Imaging.ImageConverter;

namespace Nophica.Converters
{
    class XivImageToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value?.GetType() != typeof(ImageFile))
                return null;

            ImageFile imgFile = (ImageFile) value;

            var tmp = ImageConverter.Convert(imgFile.GetData(), imgFile.Format, imgFile.Width, imgFile.Height);

            BitmapImage result;
            using (var ms = new MemoryStream())
            {
                tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = ms;
                result.EndInit();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
