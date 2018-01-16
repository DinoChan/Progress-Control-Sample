using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControlSample
{
  public enum ProgressState
    {
        Ready,
        Started,
        Completed,
        Faulted,
        Paused,
    }
}
