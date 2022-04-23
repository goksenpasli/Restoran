using System;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [Serializable]
    public enum ÖdemeŞekli
    {
        [XmlEnum(Name = "Peşin")]
        Peşin = 0,

        [XmlEnum(Name = "Kart")]
        Kart = 1,

        [XmlEnum(Name = "Fiş")]
        Fiş = 2,

        [XmlEnum(Name = "Taksitli")]
        Taksitli = 3,
    }
}