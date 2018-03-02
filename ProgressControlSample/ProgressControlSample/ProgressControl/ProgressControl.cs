using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

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
    [ContentProperty(Name = nameof(Content))]
    public partial class ProgressControl : RangeBase
    {
        /// <summary>
        ///     标识 Content 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ProgressControl), new PropertyMetadata(null, OnContentChanged));

        /// <summary>
        ///     标识 State 依赖属性。
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ProgressState), typeof(ProgressControl), new PropertyMetadata(ProgressState.Ready, OnStateChanged));

        private Button _cancelButton;

        private ProgressStateIndicator _progressStateIndicator;

        public ProgressControl()
        {
            DefaultStyleKey = typeof(ProgressControl);
        }

        /// <summary>
        ///     获取或设置Content的值
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <summary>
        ///     获取或设置State的值
        /// </summary>
        public ProgressState State
        {
            get => (ProgressState) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private static void OnContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = obj as ProgressControl;
            var oldValue = args.OldValue;
            var newValue = args.NewValue;
            if (oldValue != newValue)
                target.OnContentChanged(oldValue, newValue);
        }

        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
        }

        public event EventHandler<ProgressStateEventArgs> StateChanged;
        public event EventHandler<ProgressStateEventArgs> StateChanging;

        public event EventHandler Cancelled;

        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = obj as ProgressControl;
            var oldValue = (ProgressState) args.OldValue;
            var newValue = (ProgressState) args.NewValue;
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
            StateChanged?.Invoke(this,new ProgressStateEventArgs(oldValue,newValue));
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
            var args = new ProgressStateEventArgs(State, newstate);
            OnStateChanging(args);
            StateChanging?.Invoke(this, args);
            if (args.Cancel)
                return false;

            State = newstate;
            return true;
        }

        protected virtual void OnStateChanging(ProgressStateEventArgs args)
        {
            
        }
    }
}