using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace ProgressControlSample
{
    public sealed partial class BasicPage : UserControl
    {
        private TestService _faultTestService;
        private TestService _testService;

        public BasicPage()
        {
            this.InitializeComponent();
            StateListView.Items.Add(new ListViewItem { Content = "Ready", Tag = ProgressState.Ready });
            StateListView.Items.Add(new ListViewItem { Content = "Started", Tag = ProgressState.Started });
            StateListView.Items.Add(new ListViewItem { Content = "Paused", Tag = ProgressState.Paused });
            StateListView.Items.Add(new ListViewItem { Content = "Completed", Tag = ProgressState.Completed });
            StateListView.Items.Add(new ListViewItem { Content = "Fault", Tag = ProgressState.Faulted });
            StateListView.SelectedIndex = 0;
            _faultTestService = new TestService();
            _testService = new TestService();
        }

        private async void ProgressControlStateChanged(object sender, ProgressStateEventArgs e)
        {
            switch (e.NewValue)
            {
                case ProgressState.Ready:
                    _testService = new TestService();
                    ProgressControl.Value = ProgressControl.Minimum;
                    break;
                case ProgressState.Started:
                    try
                    {
                        _testService.IsPaused = false;
                        _testService.ProgressChanged += (s, args) => { ProgressControl.Value = args; };
                        await _testService.Start();
                        if (_testService.IsCompleted)
                            ProgressControl.State = ProgressState.Completed;
                    }
                    catch (Exception)
                    {
                        ProgressControl.State = ProgressState.Faulted;
                    }
                    break;
                case ProgressState.Paused:
                    _testService.IsPaused = true;
                    break;
                case ProgressState.Completed:
                    break;
                case ProgressState.Faulted:
                    break;
            }
        }



        private async void FaultProgressControlStateChanged(object sender, ProgressStateEventArgs e)
        {
            switch (e.NewValue)
            {
                case ProgressState.Ready:
                    _faultTestService = new TestService();
                    FaultProgressControl.Value = FaultProgressControl.Minimum;
                    break;
                case ProgressState.Started:
                    try
                    {
                        _faultTestService.IsPaused = false;
                        _faultTestService.ProgressChanged += (s, args) => { FaultProgressControl.Value = args; };
                        await _faultTestService.Start(true);
                        if (_faultTestService.IsCompleted)
                            FaultProgressControl.State = ProgressState.Completed;
                    }
                    catch (Exception)
                    {
                        FaultProgressControl.State = ProgressState.Faulted;
                    }
                    break;
                case ProgressState.Paused:
                    _faultTestService.IsPaused = true;
                    break;
                case ProgressState.Completed:
                    break;
                case ProgressState.Faulted:
                    break;
            }
        }
    }
}
