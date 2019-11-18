using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LightControl.Models
{
    public class Config
    {
        #region PresetsConfig
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public ICollection<Preset> Presets { get; set; } = new List<Preset>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public Dictionary<string, int> TransitionTimes { get; set; } = new Dictionary<string, int>();
        public string Default { get; set; }
        #endregion

        #region LampsConfig
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public ICollection<Lamp> Lamps { get; set; } = Array.Empty<Lamp>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public ICollection<EurolitePMB8> EurolitePMB8s { get; set; } = Array.Empty<EurolitePMB8>();
        public Key AddSelectionKey { get; set; }
        public Key ToogleSelectionKey { get; set; }
        public Key RemoveSelectionKey { get; set; }
        #endregion

        #region GroupsConfig
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein", Justification = "JSON Parser braucht schreibrechte")]
        public ICollection<Group> Groups { get; set; } = Array.Empty<Group>();
        #endregion
    }
}
