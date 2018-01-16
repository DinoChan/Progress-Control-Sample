using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControlSample
{
    public partial class ProgressControl
    {
        private const string CommonStatesGroupName = "CommonStates";
        private const string DeterminateStateName = "Determinate";
        private const string IndeterminateStateName = "Indeterminate";
        private const string UpdatingStateName = "Updating";
        private const string ErrorStateName = "Error";
        private const string PausedStateName = "Paused";

        private const string StartButtonName = "StartButton";
        private const string PauseButtonName = "PauseButton";
        private const string CancelButtonName = "CancelButton";
    }
}
