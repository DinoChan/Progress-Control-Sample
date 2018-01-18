using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public partial class ProgressStateIndicator : Button
    {
        public ProgressStateIndicator()
        {
            this.DefaultStyleKey = typeof(ProgressStateIndicator);
        }

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
            DependencyProperty.Register("State", typeof(ProgressState), typeof(ProgressStateIndicator), new PropertyMetadata(ProgressState.Ready, OnStateChanged));

        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ProgressStateIndicator target = obj as ProgressStateIndicator;
            ProgressState oldValue = (ProgressState)args.OldValue;
            ProgressState newValue = (ProgressState)args.NewValue;
            if (oldValue != newValue)
                target.OnStateChanged(oldValue, newValue);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualStates(false);
        }

        protected virtual void OnStateChanged(ProgressState oldValue, ProgressState newValue)
        {
            UpdateVisualStates(true);
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
    }
}
