// Utilities/Converters.cs
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AlarmCompanyManager.Models;

namespace AlarmCompanyManager.Utilities
{
    // String to Visibility Converter
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Count to Visibility Converter (shows element when count is 0)
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Boolean to String Converter
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString)
            {
                var parts = paramString.Split('|');
                if (parts.Length == 2)
                {
                    return boolValue ? parts[0] : parts[1];
                }
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Inverted Boolean to Visibility Converter
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Status to Color Converter
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int statusId)
            {
                var color = statusId switch
                {
                    (int)WorkOrderStatusEnum.Unscheduled => "#FF6B6B",
                    (int)WorkOrderStatusEnum.Scheduled => "#4ECDC4",
                    (int)WorkOrderStatusEnum.InProgress => "#45B7D1",
                    (int)WorkOrderStatusEnum.Pending => "#FFA726",
                    (int)WorkOrderStatusEnum.Canceled => "#78909C",
                    (int)WorkOrderStatusEnum.Completed => "#66BB6A",
                    _ => "#9E9E9E"
                };
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Customer Type to Color Converter
    public class CustomerTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int customerTypeId)
            {
                var color = customerTypeId switch
                {
                    (int)CustomerTypeEnum.Residential => "#2196F3",
                    (int)CustomerTypeEnum.Commercial => "#FF9800",
                    (int)CustomerTypeEnum.Government => "#4CAF50",
                    (int)CustomerTypeEnum.Education => "#9C27B0",
                    _ => "#9E9E9E"
                };
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Phone Format Converter
    public class PhoneFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ValidationHelper.FormatPhoneNumber(value?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ValidationHelper.CleanPhoneNumber(value?.ToString());
        }
    }

    // Boolean to Visibility Converter with Parameter
    public class BooleanToVisibilityConverterWithParameter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter?.ToString() == "Inverted")
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Null to Visibility Converter
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Multi-Value Converter for Customer Display Name
    public class CustomerDisplayNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3)
            {
                var firstName = values[0]?.ToString();
                var lastName = values[1]?.ToString();
                var companyName = values[2]?.ToString();

                if (!string.IsNullOrWhiteSpace(companyName))
                {
                    return $"{companyName} ({firstName} {lastName})";
                }
                return $"{firstName} {lastName}";
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Boolean to Color Converter
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return new SolidColorBrush(boolValue ? Colors.Green : Colors.Red);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Currency Converter
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("C2");
            }
            if (value is double doubleValue)
            {
                return doubleValue.ToString("C2");
            }
            if (value is float floatValue)
            {
                return floatValue.ToString("C2");
            }
            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value?.ToString()?.Replace("$", "").Replace(",", ""), out var result))
            {
                return result;
            }
            return 0m;
        }
    }

    // Date Format Converter
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var format = parameter?.ToString() ?? "MM/dd/yyyy";
                return dateTime.ToString(format);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParse(value?.ToString(), out var result))
            {
                return result;
            }
            return DateTime.MinValue;
        }
    }
}