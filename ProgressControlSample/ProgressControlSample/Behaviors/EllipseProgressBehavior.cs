using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ProgressControlSample
{
    public class EllipseProgressBehavior : Behavior<Ellipse>
    {
        /// <summary>
        /// 获取或设置Value的值
        /// </summary>  
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 标识 Value 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(EllipseProgressBehavior), new PropertyMetadata(0d, OnValueChanged));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EllipseProgressBehavior target = obj as EllipseProgressBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnValueChanged(oldValue, newValue);
        }

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            UpdateStrokeDashArray();
        }

        /// <summary>
        /// 获取或设置Maximum的值
        /// </summary>  
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// 标识 Maximum 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(EllipseProgressBehavior), new PropertyMetadata(100d, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EllipseProgressBehavior target = obj as EllipseProgressBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnMaximumChanged(oldValue, newValue);
        }

        protected virtual void OnMaximumChanged(double oldValue, double newValue)
        {
            UpdateStrokeDashArray();
        }

        /// <summary>
        /// 获取或设置Minimum的值
        /// </summary>  
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 标识 Minimum 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(EllipseProgressBehavior), new PropertyMetadata(0d, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EllipseProgressBehavior target = obj as EllipseProgressBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnMinimumChanged(oldValue, newValue);
        }

        protected virtual void OnMinimumChanged(double oldValue, double newValue)
        {
            UpdateStrokeDashArray();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateStrokeDashArray();
        }

        protected virtual double GetTotalLength()
        {
            if (AssociatedObject == null)
                return 0;

            return (AssociatedObject.ActualHeight - AssociatedObject.StrokeThickness) * Math.PI;
        }


        private void UpdateStrokeDashArray()
        {
            if (AssociatedObject == null || AssociatedObject.StrokeThickness == 0)
                return;

            var totalLength = GetTotalLength();
            totalLength = totalLength / AssociatedObject.StrokeThickness;
            var total = Maximum - Minimum;
            if (total <= 0)
                total = 1;

            var progress = (Value - Minimum) / total;
            var section = progress * totalLength;
            var result = new DoubleCollection { section, double.MaxValue };
            AssociatedObject.StrokeDashArray = result;
        }
    }
}
