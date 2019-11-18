using LightControl.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LightControl.Models
{
    public class EurolitePMB8 : Lamp
    {
        private PMB8ChannelMode? mode;
        public PMB8ChannelMode? Mode
        {
            get { return mode; }
            set { SetProperty(ref mode, value); }
        }

        private Color[] colors;
        public Color[] Colors
        {
            get { return colors; }
            set { SetProperty(ref colors, value); }
        }

        private byte speed;
        public byte Speed
        {
            get { return speed; }
            set { SetProperty(ref speed, value); }
        }

        private byte flash;
        public byte Flash
        {
            get { return flash; }
            set { SetProperty(ref flash, value); }
        }

        private byte sensitivity;
        public byte Sensitivity
        {
            get { return sensitivity; }
            set { SetProperty(ref sensitivity, value); }
        }

        public override byte[] ToDmxValues()
        {
            var colorChannels = Colors.SelectMany(i => new byte[] { i.R, i.G, i.B });
            return new byte[]
            {
                (byte)Mode,
                ((int?)Mode < 136)? Strenght :
                ((int)Mode < 240)? Speed:Sensitivity,
                Flash
            }.Concat(colorChannels).ToArray();
        }

        public override void ApplyPreset(IPresetAware from, IPresetAware to, double percentage)
        {
            if (from is EurolitePMB8 ledFrom && to is EurolitePMB8 ledTarget && !Locked)
            {
                Strenght = (byte)(ledFrom.Strenght + (ledTarget.Strenght - ledFrom.Strenght) * percentage);
                Sensitivity = (byte)(ledFrom.Sensitivity + (ledTarget.Sensitivity - ledFrom.Sensitivity) * percentage);
                Speed = (byte)(ledFrom.Speed + (ledTarget.Speed - ledFrom.Speed) * percentage);
                Flash = (byte)(ledFrom.Flash + (ledTarget.Flash - ledFrom.Flash) * percentage);
                Mode = ledTarget.Mode;

                int numberOfColors = Colors.Length;
                Color[] tmpColors = new Color[numberOfColors];
                for (int i = 0; i < numberOfColors; i++)
                    tmpColors[i] = Color.Add(ledFrom.Colors[i], Color.Multiply(Color.Subtract(ledTarget.Colors[i], ledFrom.Colors[i]), (float)percentage));
                Colors = tmpColors;
            }
            else
            {
                base.ApplyPreset(from, to, percentage);
            }
        }
    }
}
