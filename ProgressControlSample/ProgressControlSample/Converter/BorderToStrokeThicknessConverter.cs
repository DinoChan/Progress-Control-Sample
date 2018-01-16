using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ProgressControlSample
{
    public class BorderToStrokeThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var thickness = (Thickness)value;
            var max = Math.Max(thickness.Left, thickness.Top);
            max = Math.Max(max, thickness.Right);
            max = Math.Max(max, thickness.Bottom);
            return max;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
