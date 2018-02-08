using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControlSample
{
    public class ProgressStateChangingEventArgs : EventArgs
    {
        public ProgressStateChangingEventArgs(ProgressState oldValue, ProgressState newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool Cancel { get; set; }

        public ProgressState OldValue { get; }

        public ProgressState NewValue { get; }
    }
}
