using AudioPipe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.AppService;

namespace AudioPipe
{
    class TrayIcon : IDisposable
    {
        public event Action Invoked = delegate {};

        private readonly System.Windows.Forms.NotifyIcon _trayIcon;
        private AppServiceConnection _appServiceConnection;

        private System.Drawing.Icon _pipeInactiveIcon;
        private System.Drawing.Icon _pipeActiveIcon;

        public TrayIcon()
        {
            _trayIcon = new System.Windows.Forms.NotifyIcon();
            _trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

            // TODO: adjust size based on system DPI?
            _pipeActiveIcon = IconService.CreateIcon((int)IconService.Symbol.Headphone);
            _pipeInactiveIcon = IconService.CreateIcon((int)IconService.Symbol.Speaker);

            AddMenuItems(new List<MenuItem>
            {
                new MenuItem
                {
                    Text = AudioPipe.Properties.Resources.ContextMenuSettingsTitle,
                    Click = SettingsItem_Click,
                },
                new MenuItem
                {
                    Text = AudioPipe.Properties.Resources.ContextMenuAboutTitle,
                    Click = AboutItem_Click,
                },
                new MenuItem
                {
                    Text = AudioPipe.Properties.Resources.ContextMenuExitTitle,
                    Click = ExitItem_Click,
                },
            });

            _trayIcon.MouseClick += TrayIcon_MouseClick;
            _trayIcon.ContextMenu.Popup += ContextMenu_Popup;
            _trayIcon.Icon = _pipeInactiveIcon;
            _trayIcon.Text = "AudioPipe";
            _trayIcon.Visible = true;
        }

        public void SetPipeActive(bool active)
        {
            _trayIcon.Icon = active ? _pipeActiveIcon : _pipeInactiveIcon;
        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Invoked.Invoke();
            }
        }

        private void AddMenuItems(IEnumerable<MenuItem> items)
        {
            foreach (var itemDef in items)
            {
                var item = _trayIcon.ContextMenu.MenuItems.Add(itemDef.Text);
                if (itemDef.Click != null)
                {
                    item.Click += itemDef.Click;
                }
            }
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            if (_appServiceConnection != null)
            {
                _appServiceConnection.Dispose();
            }

            _trayIcon.Visible = false;
            _trayIcon.Dispose();

            Application.Current.Shutdown();
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _trayIcon.Dispose();
        }

        private struct MenuItem
        {
            public string Text { get; set; }
            public EventHandler Click { get; set; }
        }
    }
}
