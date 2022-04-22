using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Masalar")]
    public class Masalar : BaseModel
    {
        [XmlAttribute(AttributeName = "Boy")]
        public int Boy { get; set; }

        [XmlAttribute(AttributeName = "En")]
        public int En { get; set; }

        [XmlElement(ElementName = "Masa")]
        public ObservableCollection<Masa> Masa { get; set; } = new();

        [XmlIgnore]
        public bool MasaDurumunuGöster { get; set; }

        [XmlAttribute(AttributeName = "SalonAdı")]
        public string SalonAdı { get; set; }

        [XmlIgnore]
        public Masa SeçiliMasa { get; set; }
    }
}