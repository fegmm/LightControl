using LightControl.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightControl.Models
{
    public class Lamp : BindableBase, IPresetAware
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private MahApps.Metro.IconPacks.PackIconMaterialKind icon = MahApps.Metro.IconPacks.PackIconMaterialKind.Lamp;
        public MahApps.Metro.IconPacks.PackIconMaterialKind Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        private int posX;
        public int PosX
        {
            get { return posX; }
            set { SetProperty(ref posX, value); }
        }

        private int posY;
        public int PosY
        {
            get { return posY; }
            set { SetProperty(ref posY, value); }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        private bool highlighted;
        public bool Highlighted
        {
            get { return highlighted; }
            set { SetProperty(ref highlighted, value); }
        }

        private byte strenght;
        public byte Strenght
        {
            get { return strenght; }
            set { SetProperty(ref strenght, value); this.RaisePropertyChanged(nameof(Brightness)); }
        }

        private byte channel;
        public byte Channel
        {
            get { return channel; }
            set { SetProperty(ref channel, value); }
        }

        private double factor = 1;
        public double Factor
        {
            get { return factor; }
            set { SetProperty(ref factor, value); this.RaisePropertyChanged(nameof(Brightness)); }
        }

        public double Brightness => Strenght * Factor;

        private bool groupHighlighted;
        public bool GroupHighlighted
        {
            get { return groupHighlighted; }
            set { SetProperty(ref groupHighlighted, value); }
        }

        private bool locked;
        public bool Locked
        {
            get { return locked; }
            set { SetProperty(ref locked, value); }
        }

        public virtual byte[] ToDmxValues()
        {
            return new byte[] { (byte)(strenght * factor) };
        }

        public virtual void ApplyPreset(IPresetAware from, IPresetAware to, double percentage)
        {
            if (from is Lamp lampFrom && to is Lamp lampTarget && !Locked)
            {
                Strenght = (byte)(lampFrom.Strenght + (lampTarget.Strenght - lampFrom.Strenght) * percentage);
            }
        }

        public IPresetAware GetCopy()
        {
            return this.MemberwiseClone() as IPresetAware;
        }
    }
}
