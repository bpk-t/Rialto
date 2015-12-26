using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Rialto.Components.Views
{
    public class RestorableWindow : Window
    {
        #region WindowSettings 依存関係プロパティ

        public IWindowSettings WindowSettings
        {
            get { return (IWindowSettings)this.GetValue(WindowSettingsProperty); }
            set { this.SetValue(WindowSettingsProperty, value); }
        }
        public static readonly DependencyProperty WindowSettingsProperty =
            DependencyProperty.Register("WindowSettings", typeof(IWindowSettings), typeof(RestorableWindow), new UIPropertyMetadata(null));

        #endregion

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);


            if (this.WindowSettings.Placement.HasValue)
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                var placement = this.WindowSettings.Placement.Value;
                placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                placement.flags = 0;
                placement.showCmd = (placement.showCmd == SW.SHOWMINIMIZED) ? SW.SHOWNORMAL : placement.showCmd;

                Win32APIs.SetWindowPlacement(hwnd, ref placement);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!e.Cancel)
            {
                WINDOWPLACEMENT placement;
                var hwnd = new WindowInteropHelper(this).Handle;
                Win32APIs.GetWindowPlacement(hwnd, out placement);

                this.WindowSettings.Placement = placement;
                this.WindowSettings.Save();
            }
        }
    }
}
