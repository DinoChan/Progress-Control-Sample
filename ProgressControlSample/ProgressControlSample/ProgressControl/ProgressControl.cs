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
    [TemplateVisualState(GroupName = CommonStatesGroupName, Name = DeterminateStateName)]
    [TemplateVisualState(GroupName = CommonStatesGroupName, Name = IndeterminateStateName)]
    [TemplateVisualState(GroupName = CommonStatesGroupName, Name = UpdatingStateName)]
    [TemplateVisualState(GroupName = CommonStatesGroupName, Name = ErrorStateName)]
    [TemplateVisualState(GroupName = CommonStatesGroupName, Name = PausedStateName)]
    public partial class ProgressControl : ProgressBar
    {
        public ProgressControl()
        {
            this.DefaultStyleKey = typeof(ProgressControl);
        }
    }
}
