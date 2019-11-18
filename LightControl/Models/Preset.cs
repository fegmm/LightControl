using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LightControl.Models
{
    public class Preset : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private Key key;
        public Key Key
        {
            get { return key; }
            set { SetProperty(ref key, value); }
        }

        private Dictionary<string, Lamp> lampValues;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public Dictionary<string, Lamp> LampValues
        {
            get { return lampValues; }
            set { SetProperty(ref lampValues, value); }
        }

        private Dictionary<string, Group> groupValues;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public Dictionary<string, Group> GroupValues
        {
            get { return groupValues; }
            set { SetProperty(ref groupValues, value); }
        }
    }
}
