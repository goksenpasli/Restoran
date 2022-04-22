﻿using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Masa")]
    public class Masa : BaseModel
    {
        [XmlAttribute(AttributeName = "Dolu")]
        public bool Dolu { get; set; }

        [XmlAttribute(AttributeName = "Gizli")]
        public bool Gizli { get; set; }

        [XmlAttribute(AttributeName = "Kapasite")]
        public int Kapasite { get; set; }

        [XmlIgnore]
        public bool MasaToplamSiparişDurumuGöster { get; set; }

        [XmlAttribute(AttributeName = "No")]
        public int No { get; set; }

        [XmlAttribute(AttributeName = "Renk")]
        public string Renk { get; set; }

        [XmlElement(ElementName = "Siparişler")]
        public ObservableCollection<Siparişler> Siparişler { get; set; } = new();
    }
}