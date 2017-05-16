
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Code must run on UI thread.", Scope = "member", Target = "~M:AudioPipe.MainWindow.ShowAsync~System.Threading.Tasks.Task")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Code must run on UI thread.", Scope = "member", Target = "~M:AudioPipe.MainWindow.HideAsync~System.Threading.Tasks.Task")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Code must run on UI thread.", Scope = "member", Target = "~M:AudioPipe.MainWindow.Window_Deactivated(System.Object,System.EventArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Code must run on UI thread.", Scope = "member", Target = "~M:AudioPipe.MainWindow.TrayIcon_Invoked")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Code must run on UI thread.", Scope = "member", Target = "~M:AudioPipe.MainWindow.Window_PreviewKeyDown(System.Object,System.Windows.Input.KeyEventArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "AudioPipe.Services.IconService.#CreateIconFromBitmap(System.Drawing.Bitmap)")]

