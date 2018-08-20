using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// Provides an implementation of <see cref="INotifyPropertyChanged"/>
    /// which may be used by derived classes.
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
