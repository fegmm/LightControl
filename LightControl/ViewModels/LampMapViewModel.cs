using LightControl.Events;
using LightControl.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace LightControl.ViewModels
{
    public class LampMapViewModel : BindableBase
    {
        public ObservableCollection<Lamp> Lamps { get; set; }
        public DelegateCommand<IEnumerable<object>> SelectionCommand { get; set; }
        public DelegateCommand<object> OpenEditCommand { get; set; }
        public DelegateCommand ToggleLockOnSelection { get; set; }
        public DelegateCommand<object> EditLamp { get; set; }

        private SelectionMode selectionMode;
        private Key addSelectionKey;
        private Key removeSelectionKey;
        private Key toggleSelectionKey;

        public SelectionMode SelectionMode
        {
            get { return selectionMode; }
            set { SetProperty(ref selectionMode, value); }
        }

        public LampMapViewModel(Config config, IEventAggregator ea, IDialogService dialogService)
        {
            Lamps = new ObservableCollection<Lamp>(config.Lamps.Union(config.EurolitePMB8s));
            ToggleLockOnSelection = new DelegateCommand(ExecuteToggleLockOnSelection);
            SelectionCommand = new DelegateCommand<IEnumerable<object>>(ExecuteSelection);
            OpenEditCommand = new DelegateCommand<object>(o => { if (o is Lamp l) dialogService.OpenLampEdit(l); });
            SelectionMode = SelectionMode.Normal;
            ea.GetEvent<MouseWheelEvent>().Subscribe(HandleMouseWheel);
            ea.GetEvent<KeyEvent>().Subscribe(SetSelectionMode);
            addSelectionKey = config.AddSelectionKey;
            removeSelectionKey = config.RemoveSelectionKey;
            toggleSelectionKey = config.ToogleSelectionKey;
        }

        private void ExecuteToggleLockOnSelection()
        {
            switch (SelectionMode)
            {
                case SelectionMode.Add:
                    foreach (var lamp in Lamps.Where(i => i.Highlighted && !i.Locked))
                        lamp.Locked = true;
                    break;
                case SelectionMode.Remove:
                    foreach (var lamp in Lamps.Where(i => i.Highlighted && i.Locked))
                        lamp.Locked = false;
                    break;
                case SelectionMode.Toggle:
                case SelectionMode.Normal:
                    foreach (var lamp in Lamps.Where(i => i.Highlighted))
                        lamp.Locked = !lamp.Locked;
                    break;
            }
        }

        private void SetSelectionMode(KeyEventArgs e)
        {
            if (e.IsDown)
            {
                if (e.Key == Key.LeftShift)
                    selectionMode = SelectionMode.Add;
                else if (e.Key == Key.LeftAlt)
                    selectionMode = SelectionMode.Remove;
                else if (e.Key == Key.LeftCtrl)
                    selectionMode = SelectionMode.Toggle;
            }
            else if (e.IsUp)
            {
                if (e.Key == Key.LeftShift && SelectionMode == SelectionMode.Add
                    || e.Key == Key.LeftCtrl && SelectionMode == SelectionMode.Toggle
                    || e.Key == Key.LeftAlt && SelectionMode == SelectionMode.Remove)
                    selectionMode = SelectionMode.Normal;
            }
        }

        private void HandleMouseWheel(int obj)
        {
            var factor = obj / 20d;
            foreach (var item in Lamps.Where(i => i.Highlighted && !i.Locked))
            {
                double newVal = item.Strenght + factor;
                if (newVal > byte.MaxValue)
                    item.Strenght = 255;
                else if (newVal < byte.MinValue)
                    item.Strenght = 0;
                else
                    item.Strenght += (byte)factor;
            }
        }

        private void ExecuteSelection(IEnumerable<object> lamps)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Add:
                    foreach (var item in lamps.Cast<Lamp>())
                        item.Highlighted = true;
                    break;
                case SelectionMode.Remove:
                    foreach (var item in lamps.Cast<Lamp>())
                        item.Highlighted = false;
                    break;
                case SelectionMode.Toggle:
                    foreach (var item in lamps.Cast<Lamp>())
                        item.Highlighted = !item.Highlighted;
                    break;
                case SelectionMode.Normal:
                    foreach (var item in Lamps)
                        item.Highlighted = false;

                    foreach (var item in lamps.Cast<Lamp>())
                        item.Highlighted = true;
                    break;
            }
        }
    }

    public enum SelectionMode
    {
        Add,
        Remove,
        Toggle,
        Normal
    }
}
