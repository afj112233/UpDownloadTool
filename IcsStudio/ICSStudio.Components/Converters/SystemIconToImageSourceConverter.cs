using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSStudio.Components.View;
using System.Drawing;

namespace ICSStudio.Components.Converters
{
    public class SystemIconToImageSourceConverter : IValueConverter
    {
        private readonly Dictionary<Tuple<SystemIconType, int>, ImageSource> _cachedSources =
            new Dictionary<Tuple<SystemIconType, int>, ImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            SystemIconType systemIconType = (SystemIconType) value;
            ImageSource imageSource;
            int num;
            try
            {
                num = System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                num = 0;
            }
            catch (InvalidCastException)
            {
                num = 0;
            }
            catch (FormatException)
            {
                num = 0;
            }

            Tuple<SystemIconType, int> key = Tuple.Create(systemIconType, num);
            if (!this._cachedSources.TryGetValue(key, out imageSource))
            {
                Int32Rect empty = Int32Rect.Empty;
                BitmapSizeOptions sizeOptions = key.Item2 != 0
                    ? BitmapSizeOptions.FromWidthAndHeight(key.Item2, key.Item2)
                    : BitmapSizeOptions.FromEmptyOptions();
                switch (key.Item1)
                {
                    case SystemIconType.Application:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Application.Handle,
                                empty, sizeOptions);
                        break;
                    case SystemIconType.Asterisk:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Asterisk.Handle,
                                empty, sizeOptions);
                        break;
                    case SystemIconType.Error:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, empty,
                                sizeOptions);
                        break;
                    case SystemIconType.Exclamation:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Exclamation.Handle,
                                empty, sizeOptions);
                        break;
                    case SystemIconType.Hand:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Hand.Handle, empty,
                                sizeOptions);
                        break;
                    case SystemIconType.Information:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle,
                                empty, sizeOptions);
                        break;
                    case SystemIconType.Question:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle,
                                empty, sizeOptions);
                        break;
                    case SystemIconType.Shield:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Shield.Handle, empty,
                                sizeOptions);
                        break;
                    case SystemIconType.Warning:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle,
                                empty, sizeOptions);
                        break;
                    case SystemIconType.WinLogo:
                        imageSource =
                            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(SystemIcons.WinLogo.Handle,
                                empty, sizeOptions);
                        break;
                }

                if (imageSource != null)
                {
                    imageSource.Freeze();
                    this._cachedSources[key] = imageSource;
                }
            }

            return imageSource ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
