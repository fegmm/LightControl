using LightControl.Interfaces;
using LightControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightControl.Services
{
    public class PresetService
    {
        private readonly GroupService groupService;
        private readonly LightService lightService;
        private readonly Dictionary<string, Preset> nameToPreset;
        private CancellationTokenSource tokenSource;

        public PresetService(Models.Config config, GroupService groupService, LightService lightService)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.groupService = groupService;
            this.lightService = lightService;
            this.tokenSource = new CancellationTokenSource();
            this.nameToPreset = new Dictionary<string, Preset>();

            LoadPresetNames(config);
            ActivateDefaultPreset(config);
        }



        public void ActivatePreset(string presetName) => ActivatePreset(nameToPreset[presetName]);
        public void ActivatePreset(Preset preset, double milliseconds = 0)
        {
            if (preset is null)
            {
                throw new ArgumentNullException(nameof(preset));
            }

            Group defaultGroup = new Group() { Percentage = 100 };
            IEnumerable<(string name, IPresetAware oldValue, Lamp newValue)> lamps = GetPresetValuesForLamps(preset);

            var groups = preset.GroupValues
                .Union(groupService.GetGroupsNotNamed(preset.GroupValues.Keys)
                    .Select(i => new KeyValuePair<string, Group>(i, defaultGroup))
                )
                .Select(i => (name: i.Key, oldValue: groupService.GetTransitionable(i.Key), newValue: i.Value))
                .ToList();

            groups = groups
                .Select(i => i.name)
                .Distinct()
                .Select(i => groups.FirstOrDefault(j => j.name == i))
                .ToList();


            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
            }
            else
            {
                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();
            }

            Task.Run(() =>
            {
                DateTime start = DateTime.Now;
                double percentage = 0;
                do
                {
                    Thread.Sleep(10);
                    percentage = Math.Min((DateTime.Now - start).TotalMilliseconds / milliseconds, 1);
                    foreach (var lamp in lamps)
                    {
                        lightService.ApplyPreset(lamp.name, lamp.oldValue, lamp.newValue, percentage);
                    }
                    foreach (var group in groups)
                    {
                        groupService.ApplyPreset(group.name, group.oldValue, group.newValue, percentage);
                    }
                } while (percentage < 1 && !tokenSource.IsCancellationRequested);
            }, tokenSource.Token);
        }

        private IEnumerable<(string name, IPresetAware oldValue, Lamp newValue)> GetPresetValuesForLamps(Preset preset)
        {
            Lamp defaultLamp = new Lamp() { Strenght = 0 };
            var lamps = preset.LampValues.Keys
                                .Where(i => groupService.IsNameExisting(i))
                                .SelectMany(i => groupService.GetLampsInGroup(i).Select(j => (name: j, value: preset.LampValues[i])))
                                .Union(preset.LampValues.Keys.Where(i => lightService.IsNameExisting(i)).Select(j => (name: j, value: preset.LampValues[j])))
                                .Select(i => (name: i.name, oldValue: lightService.GetTransitionable(i.name), newValue: i.value));

            lamps = lamps
                .Union(lightService.GetLampsNotNamed(lamps.Select(i => i.name))
                    .Select(i => (i, lightService.GetTransitionable(i), defaultLamp))
                ).ToList();

            lamps = lamps
                .Select(i => i.name)
                .Distinct()
                .Select(i => lamps.FirstOrDefault(j => j.name == i))
                .ToList();
            return lamps;
        }

        private void LoadPresetNames(Models.Config config)
        {
            foreach (var preset in config.Presets)
            {
                nameToPreset[preset.Name] = preset;
            }
        }
        private void ActivateDefaultPreset(Models.Config config)
        {
            if (config.Default != null && nameToPreset.TryGetValue(config.Default, out Preset p))
                ActivatePreset(p, 0);
        }
    }
}
