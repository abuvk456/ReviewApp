using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommonModel;

namespace ReviewApp.Model
{

  public class BoolToColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool b && b)
      {
        return Color.Parse("Blue");
      }
      else
      {
        return Color.Parse("Transparent");
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class TopicTypeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TopicType topicType && parameter is string str)
            {
                return topicType.ToString() == str;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && parameter is string str && isChecked)
            {
                return Enum.Parse(typeof(TopicType), str);
            }

            return TopicType.Other;
        }
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible && isVisible)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
  public static class Extensions
  {
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
            try
            {
                if (source != null)
                   

                return new ObservableCollection<T>(source);
                return new ObservableCollection<T>();
            }
            catch
            {
                return new ObservableCollection<T>(source);
            }
    }
  }


}

