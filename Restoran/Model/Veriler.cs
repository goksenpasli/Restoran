using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Veriler")]
    public class Veriler : BaseModel
    {
        [XmlElement(ElementName = "Kategoriler")]
        public Kategoriler Kategoriler { get; set; }

        [XmlElement(ElementName = "Müşteriler")]
        public Müşteriler Müşteriler { get; set; }

        [XmlElement(ElementName = "Salonlar")]
        public Salonlar Salonlar { get; set; }

        [XmlElement(ElementName = "Tahsilatlar")]
        public Tahsilatlar Tahsilatlar { get; set; }

        [XmlElement(ElementName = "Ürünler")]
        public Ürünler Ürünler { get; set; }
    }
}