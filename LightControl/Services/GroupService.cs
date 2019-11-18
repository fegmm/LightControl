using LightControl.Interfaces;
using LightControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightControl.Services
{
    public class GroupService
    {
        private readonly LightService lightService;
        private readonly Dictionary<string, Group> nameToGroup;
        private readonly Dictionary<Group, int> groupValues;
        private readonly Dictionary<Group, IEnumerable<string>> groupToLamps;

        public GroupService(Config config, LightService lightService)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            if(lightService is null)
                throw new ArgumentNullException(nameof(lightService));

            this.lightService = lightService;
            this.nameToGroup = new Dictionary<string, Group>();
            this.groupValues = new Dictionary<Group, int>();
            this.groupToLamps = new Dictionary<Group, IEnumerable<string>>();

            foreach (var group in config.Groups)
            {
                nameToGroup[group.Name] = group;
                groupValues[group] = group.Percentage;
                foreach (var lamp in group.Lamps)
                {
                    if (lightService.IsNameExisting(lamp))
                        lightService.UpdateFactor(lamp, group.Percentage / 100d, 1);
                }
            }

            foreach (var group in config.Groups)
            {
                groupToLamps[group] = GetLampsInGroupHelper(group.Name, new List<string>()).Distinct();
            }
        }

        public void SetPercentage(string name, int percentage) => SetPercentage(nameToGroup[name], percentage);
        public void SetPercentage(Group group, int percentage)
        {
            if (group is null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (!group.Locked)
            {
                foreach (var item in groupToLamps[group])
                {
                    lightService.UpdateFactor(item, percentage / 100d, groupValues[group] / 100d);
                }
                groupValues[group] = percentage;
                group.Percentage = percentage;
            }
        }

        public void HighlightGroup(string groupName) => HighlightGroup(nameToGroup[groupName]);

        public IPresetAware GetTransitionable(string key)
        {
            return nameToGroup[key];
        }

        public void HighlightGroup(Group group)
        {
            if (group is null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            foreach (var lamp in group.Lamps)
            {
                lightService.HighlightLamp(lamp);
            }
            foreach (var subGroup in group.Groups)
            {
                HighlightGroup(subGroup);
            }
        }

        public void UnhighlightGroup(string groupName) => UnhighlightGroup(nameToGroup[groupName]);
        public void UnhighlightGroup(Group group)
        {
            if (group is null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            foreach (var lamp in group.Lamps)
            {
                lightService.UnhighlightLamp(lamp);
            }
            foreach (var subGroup in group.Groups)
            {
                UnhighlightGroup(subGroup);
            }
        }

        public IEnumerable<string> GetLampsInGroup(string groupName) => groupToLamps[nameToGroup[groupName]];

        private List<string> GetLampsInGroupHelper(string groupName, List<string> storage)
        {
            var group = nameToGroup[groupName];
            storage.AddRange(group.Lamps);
            foreach (var item in group.Groups)
            {
                GetLampsInGroupHelper(item, storage);
            }
            return storage;
        }

        public void ApplyPreset(string name, IPresetAware oldValue, Group newValue, double percentage)
        {
            nameToGroup[name].ApplyPreset(oldValue, newValue, percentage);
        }

        public bool IsNameExisting(string groupName) => nameToGroup.ContainsKey(groupName);
        public IEnumerable<string> GetGroupsNotNamed(IEnumerable<string> groupNames) => nameToGroup.Keys.Except(groupNames);
        public int GetPercentage(string groupName) => nameToGroup[groupName].Percentage;
    }

}
