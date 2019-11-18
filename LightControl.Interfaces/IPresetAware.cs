using System;

namespace LightControl.Interfaces
{
    public interface IPresetAware
    {
        void ApplyPreset(IPresetAware from, IPresetAware to, double percentage);
        IPresetAware GetCopy();
    }
}
