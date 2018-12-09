using AudioPipe.Properties;
using NAudio.CoreAudioApi;
using System;

namespace AudioPipe.Services
{
    /// <summary>
    /// Gets readable descriptions of HRESULT error codes.
    /// </summary>
    public static class HResultService
    {
        private enum AudioClientError
        {
            AUDCLNT_E_NOT_INITIALIZED = -2004287487,
            AUDCLNT_E_ALREADY_INITIALIZED = -2004287486,
            AUDCLNT_E_WRONG_ENDPOINT_TYPE = -2004287485,
            AUDCLNT_E_DEVICE_INVALIDATED = -2004287484,
            AUDCLNT_E_NOT_STOPPED = -2004287483,
            AUDCLNT_E_BUFFER_TOO_LARGE = -2004287482,
            AUDCLNT_E_OUT_OF_ORDER = -2004287481,
            AUDCLNT_E_UNSUPPORTED_FORMAT = -2004287480,
            AUDCLNT_E_INVALID_SIZE = -2004287479,
            AUDCLNT_E_DEVICE_IN_USE = -2004287478,
            AUDCLNT_E_BUFFER_OPERATION_PENDING = -2004287477,
            AUDCLNT_E_THREAD_NOT_REGISTERED = -2004287476,
            AUDCLNT_E_NO_SINGLE_PROCESS = -2004287475,
            AUDCLNT_E_EXCLUSIVE_MODE_NOT_ALLOWED = -2004287474,
            AUDCLNT_E_ENDPOINT_CREATE_FAILED = -2004287473,
            AUDCLNT_E_SERVICE_NOT_RUNNING = -2004287472,
            AUDCLNT_E_EVENTHANDLE_NOT_EXPECTED = -200428741,
            AUDCLNT_E_EXCLUSIVE_MODE_ONLY = -2004287470,
            AUDCLNT_E_BUFDURATION_PERIOD_NOT_EQUAL = -2004287469,
            AUDCLNT_E_EVENTHANDLE_NOT_SET = -2004287468,
            AUDCLNT_E_INCORRECT_BUFFER_SIZE = -2004287467,
            AUDCLNT_E_BUFFER_SIZE_ERROR = -2004287466,
            AUDCLNT_E_CPUUSAGE_EXCEEDED = -2004287465,
            AUDCLNT_E_BUFFER_ERROR = -2004287464,
            AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED = -2004287463,
        }

        /// <summary>
        /// Gets a message for an error related to an audio device.
        /// </summary>
        /// <param name="hresult">HRESULT of the operation.</param>
        /// <param name="device">Device relevant to the operation.</param>
        /// <returns>An error message.</returns>
        public static string GetAudioDeviceError(int hresult, MMDevice device)
        {
            if (Enum.IsDefined(typeof(AudioClientError), hresult))
            {
                switch ((AudioClientError)hresult)
                {
                    case AudioClientError.AUDCLNT_E_DEVICE_IN_USE:
                        return string.Format(Resources.ErrorDeviceBusy, device.DeviceFriendlyName);

                    default:
                        return Enum.GetName(typeof(AudioClientError), hresult);
                }
            }

            return GetGenericError(hresult);
        }

        /// <summary>
        /// Gets a generic error message with the numeric value of the HRESULT.
        /// </summary>
        /// <param name="hresult">HRESULT of the operation.</param>
        /// <returns>An error message.</returns>
        public static string GetGenericError(int hresult)
        {
            return string.Format(Resources.ErrorUnknownHresult, hresult);
        }
    }
}
