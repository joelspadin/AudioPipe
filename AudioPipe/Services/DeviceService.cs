using AudioPipe.Extensions;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides information about audio devices.
    /// </summary>
    public sealed class DeviceService : IMMNotificationClient, IDisposable
    {
        private const DataFlow CaptureDeviceDataFlow = DataFlow.Render;

        private const Role CaptureDeviceRole = Role.Console;

        private readonly MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
        private MMDevice defaultCaptureDevice;

        private DeviceService()
        {
            deviceEnum.RegisterEndpointNotificationCallback(this);
            UpdateDefaultDevice();
        }

        /// <summary>
        /// Occurs when the default audio playback device changes.
        /// </summary>
        public static event EventHandler DefaultPlaybackDeviceChanged;

        /// <summary>
        /// Occurs when the available output audio devices change.
        /// </summary>
        public static event EventHandler OutputDevicesChanged;

        /// <summary>
        /// Gets the default audio playback device.
        /// </summary>
        public static MMDevice DefaultPlaybackDevice => Instance.GetDefaultPlaybackDevice();

        private static DeviceService Instance { get; } = new DeviceService();

        /// <summary>
        /// Compares two <see cref="MMDevice"/> objects for equality.
        /// </summary>
        /// <param name="x">The first device to compare.</param>
        /// <param name="y">The second device to compare.</param>
        /// <returns>Whether the two devices are equal.</returns>
        public static bool Equals(MMDevice x, MMDevice y)
        {
            return MMDeviceEqualityComparer.Instance.Equals(x, y);
        }

        /// <summary>
        /// Gets the list of available output audio devices.
        /// </summary>
        /// <returns>A list of available output audio devices.</returns>
        public static MMDeviceCollection GetOutputDevices()
        {
            ThreadHelper.AssertOnUIThread();

            return Instance.deviceEnum.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            defaultCaptureDevice?.Dispose();
            deviceEnum.Dispose();
        }

        /// <inheritdoc/>
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == CaptureDeviceDataFlow && role == CaptureDeviceRole)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateDefaultDevice();
                    DefaultPlaybackDeviceChanged?.Invoke(this, EventArgs.Empty);
                }));
            }
        }

        /// <inheritdoc/>
        public void OnDeviceAdded(string pwstrDeviceId)
        {
            // TODO: check that this is actually an output first.
            OutputDevicesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public void OnDeviceRemoved(string deviceId)
        {
            // TODO: check that this is actually an output first.
            OutputDevicesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            // TODO: check that this is actually an output first.
            OutputDevicesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            // Nothing to do.
        }

        private MMDevice GetDefaultPlaybackDevice()
        {
            return defaultCaptureDevice;
        }

        private void UpdateDefaultDevice()
        {
            ThreadHelper.AssertOnUIThread();

            defaultCaptureDevice = deviceEnum.GetDefaultAudioEndpoint(CaptureDeviceDataFlow, CaptureDeviceRole);
            Debug.WriteLine("Default device changed to {0}", defaultCaptureDevice.FriendlyName);
        }
    }
}
