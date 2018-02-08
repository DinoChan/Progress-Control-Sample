using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ProgressControlSample
{
    public class AdjustToSquareBehavior : ProgressBehavior<Panel>
    {
        private Size _originalSize;

        /// <summary>
        /// 获取或设置ContentElement的值
        /// </summary>  
        public FrameworkElement ContentElement
        {
            get { return (FrameworkElement)GetValue(ContentElementProperty); }
            set { SetValue(ContentElementProperty, value); }
        }

        /// <summary>
        /// 标识 ContentElement 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ContentElementProperty =
            DependencyProperty.Register("ContentElement", typeof(FrameworkElement), typeof(AdjustToSquareBehavior), new PropertyMetadata(null, OnContentElementChanged));

        private static void OnContentElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AdjustToSquareBehavior target = obj as AdjustToSquareBehavior;
            FrameworkElement oldValue = (FrameworkElement)args.OldValue;
            FrameworkElement newValue = (FrameworkElement)args.NewValue;
            if (oldValue != newValue)
                target.OnContentElementChanged(oldValue, newValue);
        }

        protected virtual void OnContentElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
                newValue.SizeChanged -= OnContentElementSizeChanged;

            if (newValue != null)
            {
                newValue.SizeChanged += OnContentElementSizeChanged;
                if (Progress == 0)
                    _originalSize = newValue.RenderSize;
            }

        }

        private void OnContentElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Progress == 0)
                _originalSize = e.NewSize;

            UpdateTargetSize();
        }

        protected override void OnProgressChanged(double oldValue, double newValue)
        {
            UpdateTargetSize();
        }


        private void UpdateTargetSize()
        {
            if (ContentElement == null)
                return;

            if (AssociatedObject == null)
                return;

            if (_originalSize.Width == 0 || _originalSize.Height == 0 || double.IsNaN(Progress))
                return;

            var width = _originalSize.Width;
            var height = _originalSize.Height;
            if (width > height)
            {
                AssociatedObject.Width = width - (width - height) * Progress;
                AssociatedObject.Height = height;
            }
            else
            {
                AssociatedObject.Height = height - (height - width) * Progress;
                AssociatedObject.Width = width;
            }

        }
    }
}
