using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProgressControlSample
{
    public class RelativeOffsetBehavior : Behavior<FrameworkElement>
    {

        /// <summary>
        /// 获取或设置OffsetX的值
        /// </summary>  
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// 标识 OffsetX 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(RelativeOffsetBehavior), new PropertyMetadata(0d, OnOffsetXChanged));

        private static void OnOffsetXChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RelativeOffsetBehavior target = obj as RelativeOffsetBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnOffsetXChanged(oldValue, newValue);
        }

        protected virtual void OnOffsetXChanged(double oldValue, double newValue)
        {
            UpdateOffset();
        }


        /// <summary>
        /// 获取或设置OffsetY的值
        /// </summary>  
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// 标识 OffsetY 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(RelativeOffsetBehavior), new PropertyMetadata(0d, OnOffsetYChanged));

        private static void OnOffsetYChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RelativeOffsetBehavior target = obj as RelativeOffsetBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnOffsetYChanged(oldValue, newValue);
        }

        protected virtual void OnOffsetYChanged(double oldValue, double newValue)
        {
            UpdateOffset();
        }

        private void UpdateOffset()
        {
            if (AssociatedObject != null)
            {
                var offsetX = (float)(AssociatedObject.ActualWidth * OffsetX);
                var offsetY = (float)(AssociatedObject.ActualHeight * OffsetY);
                var animationSet = AssociatedObject.Offset(offsetX, offsetY, duration: 0, easingType: EasingType.Default);
                animationSet?.Start();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateOffset();
            if (AssociatedObject != null)
                AssociatedObject.SizeChanged += (s, e) => UpdateOffset();
        }
    }
}
