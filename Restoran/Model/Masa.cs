using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Masa")]
    public class Masa : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Dolu")]
        public bool Dolu { get; set; }    
        
        [XmlAttribute(AttributeName = "Rezerve")]
        public bool Rezerve { get; set; }

        [XmlAttribute(AttributeName = "Gizli")]
        public bool Gizli { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Kapasite")]
        public int Kapasite { get; set; }

        [XmlIgnore]
        public bool MasaToplamSiparişDurumuGöster { get; set; }

        [XmlAttribute(AttributeName = "No")]
        public int No { get; set; }

        [XmlAttribute(AttributeName = "Renk")]
        public string Renk { get; set; }

        [XmlElement(ElementName = "Rezervasyonlar")]
        public ObservableCollection<Rezervasyonlar> Rezervasyonlar { get; set; } = new();

        [XmlElement(ElementName = "Siparişler")]
        public ObservableCollection<Siparişler> Siparişler { get; set; } = new();
    }
}