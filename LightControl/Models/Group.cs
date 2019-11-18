using LightControl.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightControl.Models
{
    public class Group : BindableBase, IPresetAware
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private int percentage;
        public int Percentage
        {
            get { return percentage; }
            set { SetProperty(ref percentage, value); }
        }

        private IEnumerable<string> lamps = Array.Empty<string>();
        public IEnumerable<string> Lamps
        {
            get { return lamps; }
            set { SetProperty(ref lamps, value); }
        }

        private IEnumerable<string> groups = Array.Empty<string>();
        public IEnumerable<string> Groups
        {
            get { return groups; }
            set { SetProperty(ref groups, value); }
        }

        private bool looked;
        public bool Locked
        {
            get { return looked; }
            set { SetProperty(ref looked, value); }
        }

        private double factor;
        public double Factor
        {
            get { return factor; }
            set { SetProperty(ref factor, value); }
        }

        public void ApplyPreset(IPresetAware from, IPresetAware to, double percentage)
        {
            if (from is Group groupFrom && to is Group groupTo && !Locked)
            {
                Percentage = (byte)(groupFrom.Percentage + (groupFrom.Percentage - groupFrom.Percentage) * percentage);
            }
        }

        public IPresetAware GetCopy()
        {
            return this.MemberwiseClone() as IPresetAware;
        }
    }
}
