using AudioPipe.Services;
using NAudio.CoreAudioApi;
using System;

namespace AudioPipe.Audio
{
    /// <summary>
    /// Manages a <see cref="Pipe"/> from the default playback device to another device.
    /// </summary>
    public sealed class PipeManager : IDisposable
    {
        private int latency = Pipe.DefaultLatency;
        private bool muteInputWhenPiped;
        private Pipe pipe;

        /// <summary>
        /// Gets or sets the latency of the pipe in milliseconds.
        /// </summary>
        public int Latency
        {
            get => latency;
            set
            {
                var newLatency = Math.Max(value, Pipe.MinLatency);
                if (newLatency != latency)
                {
                    latency = newLatency;
                    Restart();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input device will be muted
        /// when the pipe is active.
        /// </summary>
        public bool MuteInputWhenPiped
        {
            get => muteInputWhenPiped;
            set
            {
                muteInputWhenPiped = value;
                if (pipe != null)
                {
                    pipe.MuteInputWhenPiped = muteInputWhenPiped;
                }
            }
        }

        /// <summary>
        /// Gets or sets the audio device to which audio will be piped.
        /// </summary>
        public MMDevice OutputDevice
        {
            get => pipe?.OutputDevice ?? DeviceService.DefaultPlaybackDevice;
            set => SetOutputDevice(value);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            pipe?.Dispose();
        }

        /// <summary>
        /// Reinitializes the pipe.
        /// </summary>
        public void Restart()
        {
            var device = OutputDevice;
            SetOutputDevice(null);
            SetOutputDevice(device);
            pipe?.Start();
        }

        /// <summary>
        /// Begins piping audio from the default playback device to <see cref="OutputDevice"/>.
        /// </summary>
        public void Start()
        {
            pipe?.Start();
        }

        /// <summary>
        /// Stops piping audio from the default playback device to <see cref="OutputDevice"/>.
        /// </summary>
        public void Stop()
        {
            pipe?.Stop();
        }

        private void SetOutputDevice(MMDevice output)
        {
            if (!Equals(output, OutputDevice))
            {
                pipe?.Dispose();

                var defaultDevice = DeviceService.DefaultPlaybackDevice;
                if (output == null || DeviceService.Equals(output, defaultDevice))
                {
                    // If there is no output or the output is the default device,
                    // we don't need to pipe anything.
                    pipe = null;
                }
                else
                {
                    pipe = new Pipe(defaultDevice, output, Latency)
                    {
                        MuteInputWhenPiped = MuteInputWhenPiped,
                    };
                }
            }
        }
    }
}
