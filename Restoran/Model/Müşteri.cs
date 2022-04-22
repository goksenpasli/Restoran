using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Müşteri")]
    public class Müşteri : BaseModel
    {
        [XmlAttribute(AttributeName = "Ad")]
        public string Ad { get; set; }

        [XmlAttribute(AttributeName = "Adres")]
        public string Adres { get; set; }

        [XmlElement(ElementName = "Sipariş")]
        public ObservableCollection<Sipariş> Sipariş { get; set; } = new();

        [XmlAttribute(AttributeName = "Soyad")]
        public string Soyad { get; set; }
    }
}