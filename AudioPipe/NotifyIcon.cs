using AudioPipe.Audio;
using AudioPipe.Properties;
using AudioPipe.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AudioPipe
{
    /// <summary>
    /// Controls the application's icon in the notification area.
    /// </summary>
    public sealed class NotifyIcon : IDisposable
    {
        private readonly System.Windows.Forms.NotifyIcon notifyIcon;
        private readonly System.Drawing.Icon pipeActiveIcon;
        private readonly System.Drawing.Icon pipeInactiveIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyIcon"/> class.
        /// </summary>
        public NotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                ContextMenu = new System.Windows.Forms.ContextMenu()
            };

            // TODO: adjust size based on system DPI?
            pipeActiveIcon = IconService.CreateIcon((int)IconService.Symbol.Headphone);
            pipeInactiveIcon = IconService.CreateIcon((int)IconService.Symbol.Speaker);

            AddMenuItems(new List<IMenuItem>
            {
                new MenuItem
                {
                    Text = Resources.ContextMenuSettingsTitle,
                    Click = SettingsItem_Click,
                },
                new MenuItem
                {
                    Text = Resources.ContextMenuHelpTitle,
                    Click = HelpItem_Click,
                },
                new Separator(),
                new MenuItem
                {
                    Text = Resources.ContextMenuExitTitle,
                    Click = ExitItem_Click,
                },
            });

            notifyIcon.MouseClick += TrayIcon_MouseClick;
            notifyIcon.Icon = pipeInactiveIcon;
            notifyIcon.Text = Resources.TrayIconText;
            notifyIcon.Visible = true;
        }

        /// <summary>
        /// Occurs when the icon is clicked or otherwise invoked.
        /// </summary>
        public event EventHandler Invoked;

        /// <summary>
        /// Occurs when the settings option in the menu is clicked.
        /// </summary>
        public event EventHandler SettingsClicked;

        /// <summary>
        /// Occurs when the help option in the menu is clicked.
        /// </summary>
        public event EventHandler HelpClicked;

        /// <summary>
        /// Describes an item of the <see cref="NotifyIcon"/>'s context menu.
        /// </summary>
        private interface IMenuItem
        {
            /// <summary>
            /// Gets an <see cref="EventHandler"/> that should be called
            /// when the menu item is clicked.
            /// </summary>
            EventHandler Click { get; }

            /// <summary>
            /// Gets the text to display for the menu item.
            /// </summary>
            string Text { get; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            notifyIcon.Dispose();
        }

        /// <summary>
        /// Changes the icon based on whether the <see cref="Pipe"/> is active.
        /// </summary>
        /// <param name="active">Whether the pipe is active.</param>
        public void SetPipeActive(bool active)
        {
            notifyIcon.Icon = active ? pipeActiveIcon : pipeInactiveIcon;
        }

        private void AddMenuItems(IEnumerable<IMenuItem> items)
        {
            foreach (var itemDef in items)
            {
                var item = notifyIcon.ContextMenu.MenuItems.Add(itemDef.Text);
                if (itemDef.Click != null)
                {
                    item.Click += itemDef.Click;
                }
            }
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Dispose();

            Application.Current.Shutdown();
        }

        private void HelpItem_Click(object sender, EventArgs e)
        {
            HelpClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            SettingsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Invoked?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An <see cref="IMenuItem"/> with text and a click handler.
        /// </summary>
        private struct MenuItem : IMenuItem
        {
            /// <inheritdoc/>
            public EventHandler Click { get; set; }

            /// <inheritdoc/>
            public string Text { get; set; }
        }

        /// <summary>
        /// An <see cref="IMenuItem"/> that creates a horizontal bar in the context menu.
        /// </summary>
        private class Separator : IMenuItem
        {
            /// <summary>
            /// Gets an event handler that does nothing when the menu item is clicked.
            /// </summary>
            public EventHandler Click => null;

            /// <summary>
            /// Gets menu item text that creates a separator in the context menu.
            /// </summary>
            public string Text => "-";
        }
    }
}
