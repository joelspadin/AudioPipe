using AudioPipe.Services;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioPipe
{
    public class Pipe : IDisposable
    {
        public const int DefaultLatency = 5;

        public MMDevice CaptureDevice { get; }
        public MMDevice OutputDevice { get; }
        public PlaybackState PlaybackState => _output?.PlaybackState ?? PlaybackState.Stopped;

        private AudioEndpointVolume _inputVolume;
        private WasapiLoopbackCapture _capture;
        private WasapiOut _output;

        public Pipe(MMDevice capture, MMDevice output, int latency = DefaultLatency)
        {
            if (DeviceService.Equals(capture, output))
            {
                throw new ArgumentException($"{nameof(capture)} and {nameof(output)} cannot both be {capture.FriendlyName}");
            }

            CaptureDevice = capture;
            OutputDevice = output;

            try
            {
                _capture = new WasapiLoopbackCapture(latency);
                _capture.Device = CaptureDevice;
                _capture.Initialize();
            }
            catch (CoreAudioAPIException ex)
            {
                throw new PipeInitException("Failed to connect to source device. Is another program using it?", ex);
            }

            try
            {
                var source = new SoundInSource(_capture) { FillWithZeros = true };

                _output = new WasapiOut(false, AudioClientShareMode.Shared, latency);
                _output.Device = OutputDevice;
                _output.Initialize(source);
            }
            catch (CoreAudioAPIException ex)
            {
                throw new PipeInitException("Failed to connect to destination device. Is another program using it?", ex);
            }

            _inputVolume = AudioEndpointVolume.FromDevice(CaptureDevice);
        }

        public void Start()
        {
            if (_isDisposed)
            {
                return;
            }

            if (PlaybackState != PlaybackState.Playing)
            {
                _capture.Start();
                _output.Play();
                _inputVolume.IsMuted = true;
            }
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                return;
            }

            if (PlaybackState == PlaybackState.Playing)
            {
                _output.Stop();
                _capture.Stop();
                _inputVolume.IsMuted = false;
            }
        }

        #region IDisposable Support
        private bool _isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Stop();

                    _capture?.Dispose();
                    _capture = null;

                    _output?.Dispose();
                    _output = null;

                    _inputVolume?.Dispose();
                    _inputVolume = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _isDisposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AudioPipe() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
