using MahApps.Metro.Controls;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LightControl.Controls
{
    class MetroDialogWindow : MahApps.Metro.Controls.MetroWindow, IDialogWindow
    {
        public IDialogResult Result { get; set; }

        protected override void OnActivated(EventArgs e)
        {
            (Application.Current.MainWindow as MetroWindow).ShowOverlayAsync();
            base.OnActivated(e);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            (Application.Current.MainWindow as MetroWindow).HideOverlayAsync();

            base.OnClosing(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(1);
                Dispatcher.Invoke(() =>
                {
                    Close();
                });
            });

            base.OnDeactivated(e);
        }
    }
}
