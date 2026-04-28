using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HelpDeskPro.Wpf.Models;

namespace HelpDeskPro.Wpf.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        TicketStatus.Open => new SolidColorBrush(Color.FromRgb(21, 101, 192)),
        TicketStatus.InProgress => new SolidColorBrush(Color.FromRgb(245, 127, 23)),
        TicketStatus.Closed => new SolidColorBrush(Color.FromRgb(46, 125, 50)),
        _ => Brushes.Gray
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}

public class StatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        TicketStatus.Open => new SolidColorBrush(Color.FromRgb(227, 242, 253)),
        TicketStatus.InProgress => new SolidColorBrush(Color.FromRgb(255, 248, 225)),
        TicketStatus.Closed => new SolidColorBrush(Color.FromRgb(232, 245, 233)),
        _ => Brushes.LightGray
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        Priority.Low => new SolidColorBrush(Color.FromRgb(100, 100, 100)),
        Priority.Medium => new SolidColorBrush(Color.FromRgb(133, 100, 4)),
        Priority.High => new SolidColorBrush(Color.FromRgb(230, 81, 0)),
        Priority.Critical => new SolidColorBrush(Color.FromRgb(198, 40, 40)),
        _ => Brushes.Gray
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is true ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is false ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is null ? Visibility.Collapsed : Visibility.Visible;
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}

public class RoleToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is UserRole.Admin ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
