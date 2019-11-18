using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace LightControl.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand<MouseWheelEventArgs> MouseWheelCommand { get; set; }
        public DelegateCommand<KeyEventArgs> KeyCommand { get; set; }

        public MainWindowViewModel(IEventAggregator ea)
        {
            MouseWheelCommand = new DelegateCommand<MouseWheelEventArgs>(e => ea.GetEvent<Events.MouseWheelEvent>().Publish(e.Delta));
            KeyCommand = new DelegateCommand<KeyEventArgs>(e => ea.GetEvent<Events.KeyEvent>().Publish(e));
        }
    }
}
