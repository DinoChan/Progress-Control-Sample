﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControlSample
{
    public partial class ProgressControl
    {
        private const string ProgressStatesGroupName = "ProgressStates";
        private const string ReadyStateName = "Ready";
        private const string StartedStateName = "Started";
        private const string CompletedStateName = "Completed";
        private const string FaultedStateName = "Faulted";
        private const string PausedStateName = "Paused";

        private const string ProgressStateIndicatorName = "ProgressStateIndicator";
        private const string CancelButtonName = "CancelButton";
    }
}
