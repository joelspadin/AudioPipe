using AudioPipe.Properties;
using AudioPipe.Services;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using System;

namespace AudioPipe.Audio
{
    /// <summary>
    /// Pipes audio from one audio device to another.
    /// </summary>
    public class Pipe : IDisposable
    {
        /// <summary>
        /// Default pipe latency in milliseconds.
        /// </summary>
        public const int DefaultLatency = 10;

        /// <summary>
        /// Minimum pipe latency in milliseconds.
        /// </summary>
        public const int MinLatency = 2;

        private WasapiLoopbackCapture inputCapture;
        private AudioEndpointVolume inputVolume;

        /// <summary>
        /// To detect redundant <see cref="Dispose()"/> calls.
        /// </summary>
        private bool isDisposed;

        private bool muteInputWhenPiped;
        private WasapiOut outputDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipe"/> class.
        /// </summary>
        /// <param name="input">Audio device from which to capture audio.</param>
        /// <param name="output">Audio device to which the captured audio should be output.</param>
        /// <param name="latency">Latency of the pipe in milliseconds. </param>
        public Pipe(MMDevice input, MMDevice output, int latency = DefaultLatency)
        {
            if (DeviceService.Equals(input, output))
            {
                throw new ArgumentException($"{nameof(input)} and {nameof(output)} cannot both be {input.FriendlyName}");
            }

            if (latency < MinLatency)
            {
                throw new ArgumentException("Latency is too low.", nameof(latency));
            }

            InputDevice = input;
            OutputDevice = output;

            try
            {
                inputCapture = new WasapiLoopbackCapture(latency / 2)
                {
                    Device = InputDevice
                };
                inputCapture.Initialize();
            }
            catch (CoreAudioAPIException ex)
            {
                throw new PipeInitException(Resources.ErrorSourceDeviceBusy, ex);
            }

            try
            {
                var source = new SoundInSource(inputCapture) { FillWithZeros = true };

                // Todo: test performance when eventSync = true.
                outputDevice = new WasapiOut(false, AudioClientShareMode.Shared, latency / 2)
                {
                    Device = OutputDevice
                };
                outputDevice.Initialize(source);
            }
            catch (CoreAudioAPIException ex)
            {
                throw new PipeInitException(Resources.ErrorDestinationDeviceBusy, ex);
            }

            inputVolume = AudioEndpointVolume.FromDevice(InputDevice);
        }

        /// <summary>
        /// Gets the device from which audio is being captured.
        /// </summary>
        public MMDevice InputDevice { get; }

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
                if (PlaybackState == PlaybackState.Playing)
                {
                    inputVolume.IsMuted = muteInputWhenPiped;
                }
            }
        }

        /// <summary>
        /// Gets the device to which the captured audio is piped.
        /// </summary>
        public MMDevice OutputDevice { get; }

        /// <summary>
        /// Gets the playback state of the pipe.
        /// </summary>
        public PlaybackState PlaybackState => outputDevice?.PlaybackState ?? PlaybackState.Stopped;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing).
            Dispose(true);
        }

        /// <summary>
        /// Begins piping audio from <see cref="InputDevice"/> to <see cref="OutputDevice"/>.
        /// Does nothing if the pipe is already started.
        /// </summary>
        public void Start()
        {
            if (isDisposed)
            {
                return;
            }

            if (PlaybackState != PlaybackState.Playing)
            {
                inputCapture.Start();
                outputDevice.Play();
                inputVolume.IsMuted = MuteInputWhenPiped;
            }
        }

        /// <summary>
        /// Stops piping audio from <see cref="InputDevice"/> to <see cref="OutputDevice"/>.
        /// Does nothing if the pipe is already stopped.
        /// </summary>
        public void Stop()
        {
            if (isDisposed)
            {
                return;
            }

            if (PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                inputCapture.Stop();
                inputVolume.IsMuted = false;
            }
        }

        /// <summary>
        /// Frees managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method was invoked by <see cref="Dispose()"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Stop();

                    inputCapture?.Dispose();
                    inputCapture = null;

                    outputDevice?.Dispose();
                    outputDevice = null;

                    inputVolume?.Dispose();
                    inputVolume = null;
                }

                isDisposed = true;
            }
        }
    }
}
