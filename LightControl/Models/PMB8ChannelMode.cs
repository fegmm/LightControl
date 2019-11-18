using System;
using System.Collections.Generic;
using System.Text;

namespace LightControl.Models
{
    public enum PMB8ChannelMode
    {
        Dimmer = 0,
        Rot = 16,
        Gelb = 24,
        Blau = 32,
        RotGelb = 40,
        GelbBlau = 48,
        RotBlau = 56,
        RotGelbBlau = 64,
        Farbe1 = 72,
        Farbe2 = 80,
        Farbe3 = 88,
        Farbe4 = 96,
        Farbe5 = 104,
        Farbe6 = 112,
        Farbe7 = 120,
        Farbe8 = 128,

        Dream = 136,
        Meteor = 144,
        Fade = 152,
        Change = 160,
        Flow1 = 168,
        Flow2 = 176,
        Flow3 = 184,
        Flow4 = 192,
        Flow5 = 200,
        Flow6 = 208,
        Flow7 = 216,
        Flow8 = 224,
        Flow9 = 232,
        Sound = 240
    }
}
