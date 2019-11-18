using LightControl.Interfaces;
using LightControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace LightControl.Services
{

    public class LightService
    {
        private readonly Dictionary<string, Models.Lamp> nameToLamp;
        private readonly DMXService dmxService;
        private readonly CancellationTokenSource dmxCancellation;

        public LightService(Config config, DMXService dmxService)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            if (dmxService == null)
                throw new ArgumentNullException(nameof(dmxService));

            this.nameToLamp = new Dictionary<string, Lamp>();

            foreach (var item in config.Lamps.Union(config.EurolitePMB8s))
            {
                nameToLamp[item.Name] = item;
                item.PropertyChanged += Lamp_PropertyChanged;
                dmxService.UpdatePeriodicalBuffer(item.Channel, item.ToDmxValues());
            }

            this.dmxService = dmxService;
            dmxService.Connect();
            dmxCancellation = dmxService.StartPeriodicalSend(10);
        }

        private void Lamp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Lamp lamp)
            {
                dmxService.UpdatePeriodicalBuffer(lamp.Channel, lamp.ToDmxValues());
            }
        }

        public void SetValues(Dictionary<string, byte> values)
        {
            if (values == null)
                return;

            foreach (var item in values)
                if (nameToLamp.TryGetValue(item.Key, out Lamp lamp))
                    if (!lamp.Locked)
                        lamp.Strenght = item.Value;
        }

        public void HighlightLamp(string lampName)
        {
            if (nameToLamp.TryGetValue(lampName, out Lamp lamp))
                lamp.GroupHighlighted = true;
        }

        public IPresetAware GetTransitionable(string name)
        {
            return nameToLamp[name].GetCopy();
        }

        public void UnhighlightLamp(string lampName)
        {
            if (nameToLamp.TryGetValue(lampName, out Lamp lamp))
                lamp.GroupHighlighted = false;
        }

        public void UpdateFactor(string lampName, double factor, double oldFactor)
        {
            if (factor == 0)
                factor = 1e-10;
            if (oldFactor == 0)
                oldFactor = 1e-10;

            if (nameToLamp.TryGetValue(lampName, out Lamp lamp))
            {
                lamp.Factor = Math.Min(1, Math.Max(1e-10, lamp.Factor * factor / oldFactor));
            }
        }

        public bool IsNameExisting(string lampName) => nameToLamp.ContainsKey(lampName);
        public IEnumerable<string> GetLampsNotNamed(IEnumerable<string> lampNames) => nameToLamp.Keys.Except(lampNames);
        public byte GetLampValue(string lampName) => nameToLamp[lampName].Strenght;

        public void ApplyPreset(string name, IPresetAware oldValue, Lamp newValue, double percentage)
        {
            nameToLamp[name].ApplyPreset(oldValue, newValue, percentage);
        }
    }
}
