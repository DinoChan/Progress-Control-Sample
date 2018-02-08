using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ProgressControlSample
{
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = ReadyStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = StartedStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = CompletedStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = FaultedStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = PausedStateName)]

    [TemplatePart(Name = ProgressStateIndicatorName, Type = typeof(ProgressStateIndicator))]
    [TemplatePart(Name = CancelButtonName, Type = typeof(Button))]
    public partial class ProgressControl : RangeBase
    {

        private ProgressStateIndicator _progressStateIndicator;
        private Button _cancelButton;

        public ProgressControl()
        {
            this.DefaultStyleKey = typeof(ProgressControl);
        }

        public event EventHandler StateChanged;
        public event EventHandler<ProgressStateChangingEventArgs> StateChanging;

        public event EventHandler Completed;
        public event EventHandler Cancelled;

        /// <summary>
        /// 获取或设置State的值
        /// </summary>  
        public ProgressState State
        {
            get { return (ProgressState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// 标识 State 依赖属性。
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ProgressState), typeof(ProgressControl), new PropertyMetadata(ProgressState.Ready, OnStateChanged));

        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ProgressControl target = obj as ProgressControl;
            ProgressState oldValue = (ProgressState)args.OldValue;
            ProgressState newValue = (ProgressState)args.NewValue;
            if (oldValue != newValue)
                target.OnStateChanged(oldValue, newValue);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _progressStateIndicator = GetTemplateChild(ProgressStateIndicatorName) as ProgressStateIndicator;
            if (_progressStateIndicator != null)
                _progressStateIndicator.Click += OnGoToNextState;

            _cancelButton = GetTemplateChild(CancelButtonName) as Button;

            if (_cancelButton != null)
                _cancelButton.Click += OnCancel;

            UpdateVisualStates(false);
        }


        private void OnGoToNextState(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case ProgressState.Ready:
                    ChangeStateCore(ProgressState.Started);
                    break;
                case ProgressState.Started:
                    ChangeStateCore(ProgressState.Paused);
                    break;
                case ProgressState.Completed:
                    ChangeStateCore(ProgressState.Ready);
                    break;
                case ProgressState.Faulted:
                    ChangeStateCore(ProgressState.Ready);
                    break;
                case ProgressState.Paused:
                    ChangeStateCore(ProgressState.Started);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            if (ChangeStateCore(ProgressState.Ready))
                Cancelled?.Invoke(this, EventArgs.Empty);
        }


        protected virtual void OnStateChanged(ProgressState oldValue, ProgressState newValue)
        {
            UpdateVisualStates(true);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (ChangeStateCore(ProgressState.Completed))
                Completed?.Invoke(this, EventArgs.Empty);
        }


        private void UpdateVisualStates(bool useTransitions)
        {
            string progressState;
            switch (State)
            {
                case ProgressState.Ready:
                    progressState = ReadyStateName;
                    break;
                case ProgressState.Started:
                    progressState = StartedStateName;
                    break;
                case ProgressState.Completed:
                    progressState = CompletedStateName;
                    break;
                case ProgressState.Faulted:
                    progressState = FaultedStateName;
                    break;
                case ProgressState.Paused:
                    progressState = PausedStateName;
                    break;
                default:
                    progressState = ReadyStateName;
                    break;
            }
            VisualStateManager.GoToState(this, progressState, useTransitions);
        }

        private bool ChangeStateCore(ProgressState newstate)
        {

            var args = new ProgressStateChangingEventArgs(this.State, newstate);
            //if (args.OldValue == ProgressState.Started && args.NewValue == ProgressState.Ready)
            //    args.Cancel = true;

            OnStateChanging(args);
            StateChanging?.Invoke(this, args);
            if (args.Cancel)
                return false;

            State = newstate;
            return true;
        }

        protected virtual void OnStateChanging(ProgressStateChangingEventArgs args)
        {

        }

    }
}
