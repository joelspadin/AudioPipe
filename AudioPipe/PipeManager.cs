using AudioPipe.Services;
using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe
{
    public class PipeManager : IDisposable
    {
        public int Latency
        {
            get => _latency;
            set
            {
                var newLatency = Math.Max(value, Pipe.MinLatency);
                if (newLatency != _latency)
                {
                    _latency = newLatency;
                    Restart();
                }
            }
        }

        private int _latency = Pipe.DefaultLatency;
        private Pipe _pipe;

        public MMDevice CurrentOutput
        {
            get => _pipe?.OutputDevice ?? DeviceService.DefaultCaptureDevice;
            set => SetOutputDevice(value);
        }

        public void Start()
        {
            _pipe?.Start();
        }

        public void Stop()
        {
            _pipe?.Stop();
        }

        public void Restart()
        {
            var device = CurrentOutput;
            SetOutputDevice(null);
            SetOutputDevice(device);
        }

        public void SetOutputDevice(MMDevice output)
        {
            if (!DeviceService.Equals(output, CurrentOutput))
            {
                _pipe?.Dispose();

                var defaultDevice = DeviceService.DefaultCaptureDevice;
                if (output == null || DeviceService.Equals(output, defaultDevice))
                {
                    _pipe = null;
                }
                else
                {
                    _pipe = new Pipe(defaultDevice, output, Latency);
                }
            }
        }

        public void Dispose()
        {
            _pipe?.Dispose();
        }

        // TODO: what happens if one of the devices gets removed?
    }
}
