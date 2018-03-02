using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControlSample
{
    public class TestService
    {
        public TestService()
        {
            _progress = 0;
        }

        private double _progress;

        public bool IsCompleted { get; private set; }

        public event EventHandler<double> ProgressChanged;

        public bool IsPaused { get; set; }

        public bool IsStarted { get; private set; }

        public async Task Start(bool throwException = false)
        {
            IsStarted = true;
            try
            {
                ProgressChanged?.Invoke(this, _progress);
                await Task.Delay(1000);
                while (_progress < 100)
                {
                    await Task.Delay(100);
                    _progress += 3;
                    ProgressChanged?.Invoke(this, _progress);
                    if (_progress > 70 && throwException)
                        throw new Exception("test");

                    if (IsPaused)
                        return;
                }

                IsCompleted = true;
            }
            finally
            {
                IsStarted = false;
            }
        }
    }
}
