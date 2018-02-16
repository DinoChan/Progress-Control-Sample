using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProgressControlSample
{
  public  class FrameworkElementExtensions
    {

        /// <summary>
        //  从指定元素获取 IsAdjustToSquare 依赖项属性的值。
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>IsAdjustToSquare 依赖项属性的值</returns>
        public static bool GetIsAdjustToSquare(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAdjustToSquareProperty);
        }

        /// <summary>
        /// 将 IsAdjustToSquare 依赖项属性的值设置为指定元素。
        /// </summary>
        /// <param name="obj">The element on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetIsAdjustToSquare(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAdjustToSquareProperty, value);
        }

        /// <summary>
        /// 标识 IsAdjustToSquare 依赖项属性。
        /// </summary>
        public static readonly DependencyProperty IsAdjustToSquareProperty =
            DependencyProperty.RegisterAttached("IsAdjustToSquare", typeof(bool), typeof(FrameworkElementExtensions), new PropertyMetadata(false, OnIsAdjustToSquareChanged));


        private static void OnIsAdjustToSquareChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = obj as FrameworkElement;
            bool oldValue = (bool)args.OldValue;
            bool newValue = (bool)args.NewValue;
            if (oldValue == newValue)
                return;

            if (newValue == false)
                return;

            var action = new Action(() =>
              {
                  var width = target.RenderSize.Width;
                  var height = target.RenderSize.Height;
                  if (double.IsInfinity(width) || double.IsInfinity(height) || width == 0 || height == 0)
                      return;

                  //if (width > height)
                  //{
                      target.Width = height;
                  //}
                  //else
                  //{
                  //    target.Height = width;
                  //}
              });

            target.SizeChanged += (s, e) =>
            {
                action();
            };
            action();
        }


    }
}
