// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
using System;
using System.Globalization;
using System.Windows.Data;

namespace AudioPipe
{
    /// <summary>
    /// <see cref="IValueConverter"/> that converts a numeric value to a
    /// string with a milliseconds unit.
    /// </summary>
    public sealed class ValueToMillisecondsConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value ?? "null"} ms";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converting from string is not supported.");
        }
    }
}
