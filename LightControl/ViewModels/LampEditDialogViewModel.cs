using LightControl.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightControl.ViewModels
{
    public class LampEditDialogViewModel : BindableBase, Prism.Services.Dialogs.IDialogAware
    {
        private Lamp lamp;
        public Lamp Lamp
        {
            get { return lamp; }
            set { SetProperty(ref lamp, value); RaisePropertyChanged(nameof(Title)); }
        }

        public IEnumerable<PMB8ChannelMode> PMB8ChannelModes { get; }
        public DelegateCommand CloseCommand { get; }

        public LampEditDialogViewModel()
        {
            PMB8ChannelModes = Enum.GetNames(typeof(PMB8ChannelMode))
                .Select(i => (PMB8ChannelMode)Enum.Parse(typeof(PMB8ChannelMode), i));

            CloseCommand = new DelegateCommand(() => RequestClose(null));
        }

        public string Title => $"Eigenschaften der Lampe: {Lamp?.Name}";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.TryGetValue(nameof(Lamp), out Lamp lamp))
            {
                this.Lamp = lamp;
            }
            else
                RequestClose(new DialogResult());
        }
    }
}
