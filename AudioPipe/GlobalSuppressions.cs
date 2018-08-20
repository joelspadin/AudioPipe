// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Lots of code must run on UI thread.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File must have header", Justification = "Don't want file headers.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Prefixing with 'this' is redundant.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "SG0001:Potential command injection with Process.Start", Justification = "Hyperlinks are not controlled by user input.", Scope = "member", Target = "~M:AudioPipe.Extensions.NaivgationExtensions.Hyperlink_RequestNavigate(System.Object,System.Windows.Navigation.RequestNavigateEventArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace must be followed by blank line", Justification = "Looks weird in switch statement.", Scope = "member", Target = "~M:AudioPipe.MainWindow.WindowHook(System.IntPtr,System.Int32,System.IntPtr,System.IntPtr,System.Boolean@)~System.IntPtr")]
