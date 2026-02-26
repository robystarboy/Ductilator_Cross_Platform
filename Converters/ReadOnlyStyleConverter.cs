using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace Ductilator.Converters;

public class ReadOnlyBackgroundConverter : IValueConverter
{
    private static readonly SolidColorBrush ReadOnlyBrush = new SolidColorBrush(Color.Parse("#F0F0F0"));
    private static readonly SolidColorBrush EditableBrush = new SolidColorBrush(Colors.White);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        if (value is bool isReadOnly && isReadOnly)
        {
            return ReadOnlyBrush;
        }
        return EditableBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        throw new NotImplementedException();
    }
}

public class ReadOnlyForegroundConverter : IValueConverter
{
    private static readonly SolidColorBrush ReadOnlyBrush = new SolidColorBrush(Color.Parse("#555555"));
    private static readonly SolidColorBrush EditableBrush = new SolidColorBrush(Colors.Black);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        if (value is bool isReadOnly && isReadOnly)
        {
            return ReadOnlyBrush;
        }
        return EditableBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        throw new NotImplementedException();
    }
}
