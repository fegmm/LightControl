using LightControl.Models;
using LightControl.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LightControl.ViewModels
{
    public class GroupsViewModel : BindableBase
    {
        public ObservableCollection<Group> Groups { get; }
        public DelegateCommand<Group> GroupValueCommand { get; set; }
        public DelegateCommand<Group> HighlightGroupCommand { get; set; }
        public DelegateCommand<Group> UnhighlightGroupCommand { get; set; }
        public DelegateCommand<Group> ToggleLockCommand { get; set; }

        public GroupsViewModel(GroupService groupService, Config config)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            Groups = new ObservableCollection<Group>(config.Groups);
            GroupValueCommand = new DelegateCommand<Group>(group => groupService.SetPercentage(group, group.Percentage));
            HighlightGroupCommand = new DelegateCommand<Group>(group => groupService.HighlightGroup(group));
            UnhighlightGroupCommand = new DelegateCommand<Group>(group => groupService.UnhighlightGroup(group));
            ToggleLockCommand = new DelegateCommand<Group>(group => group.Locked = !group.Locked);
        }
    }
}
