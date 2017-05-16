using AudioPipe.Services;
using CSCore.CoreAudioAPI;
using System;

namespace AudioPipe
{
    public sealed class PipeManager : IDisposable
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

        public bool MuteSource
        {
            get => _muteSource;
            set
            {
                _muteSource = value;
                if (_pipe != null)
                {
                    _pipe.MuteSource = _muteSource;
                }
            }
        }

        private int _latency = Pipe.DefaultLatency;
        private bool _muteSource;
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
            _pipe?.Start();
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
                    _pipe = new Pipe(defaultDevice, output, Latency)
                    {
                        MuteSource = MuteSource,
                    };
                }
            }
        }

        public void Dispose()
        {
            _pipe?.Dispose();
        }
    }
}
