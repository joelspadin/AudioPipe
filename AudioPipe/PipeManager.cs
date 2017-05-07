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
        public int Latency { get; set; }

        private Pipe _pipe;

        public MMDevice CurrentOutput
        {
            get => _pipe?.OutputDevice ?? DeviceService.DefaultCaptureDevice;
            set => SetOutputDevice(value);
        }

        public PipeManager(int latency = Pipe.DefaultLatency)
        {
            Latency = latency;
        }

        public void Start()
        {
            _pipe?.Start();
        }

        public void Stop()
        {
            _pipe?.Stop();
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
