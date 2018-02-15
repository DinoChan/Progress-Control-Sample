using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProgressControlSample
{
    public class SizeBridge : DependencyObject, INotifyPropertyChanged
    {

        /// <summary>
        /// 获取或设置Element的值
        /// </summary>  
        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        /// <summary>
        /// 标识 Element 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(SizeBridge), new PropertyMetadata(null, OnElementChanged));

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SizeBridge target = obj as SizeBridge;
            FrameworkElement oldValue = (FrameworkElement)args.OldValue;
            FrameworkElement newValue = (FrameworkElement)args.NewValue;
            if (oldValue != newValue)
                target.OnElementChanged(oldValue, newValue);
        }



        protected virtual void OnElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (newValue == null)
                return;

            newValue.SizeChanged += OnElementSizeChanged;
        }



        private double _toLeft;

        /// <summary>
        /// 获取或设置 ToLeft 的值
        /// </summary>
        public double ToLeft
        {
            get { return _toLeft; }
            set
            {
                if (_toLeft == value)
                    return;

                _toLeft = value;
                RaisePropertyChanged("ToLeft");
            }
        }


        private double _toRight;

        /// <summary>
        /// 获取或设置 ToRight 的值
        /// </summary>
        public double ToRight
        {
            get { return _toRight; }
            set
            {
                if (_toRight == value)
                    return;

                _toRight = value;
                RaisePropertyChanged("ToRight");
            }
        }

        private void OnElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ToLeft = (Element.ActualHeight * 0.85);
            ToRight = Element.ActualHeight * 0.85;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
