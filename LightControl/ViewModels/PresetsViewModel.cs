using LightControl.Events;
using LightControl.Models;
using LightControl.Services;
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
    public class PresetsViewModel : BindableBase
    {
        private readonly PresetService presetService;

        public ObservableCollection<Preset> Presets { get; }
        public Dictionary<string, int> TimeSettings { get; }
        public KeyValuePair<string, int> SelectedTime { get; set; }
        public DelegateCommand<Preset> ActivatePresetCommand { get; set; }
        public DelegateCommand OpenPresetDiagonal { get; set; }

        
        public PresetsViewModel(IDialogService dialogService, IEventAggregator ea, PresetService presetService, Config config)
        {
            if (ea is null)
                throw new ArgumentNullException(nameof(ea));
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            this.presetService = presetService;
            ea.GetEvent<KeyEvent>().Subscribe(CheckIfPresetShouldBeActivated);
            Presets = new ObservableCollection<Preset>(config.Presets);
            ActivatePresetCommand = new DelegateCommand<Preset>(i => presetService.ActivatePreset(i, SelectedTime.Value));
            TimeSettings = config.TransitionTimes;
            SelectedTime = config.TransitionTimes.FirstOrDefault();
            //OpenPresetDiagonal = new DelegateCommand(() => dialogService.)
        }

        private void CheckIfPresetShouldBeActivated(KeyEventArgs obj)
        {
            if (obj.IsDown && Presets.FirstOrDefault(i => i.Key == obj.Key) is Preset p)
                presetService.ActivatePreset(p, SelectedTime.Value);
        }
    }
}
