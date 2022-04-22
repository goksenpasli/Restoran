using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Tahsilat")]
    public class Tahsilat : BaseModel
    {
        [XmlAttribute(AttributeName = "SiparişlerId")]
        public int SiparişlerId { get; set; }

        [XmlAttribute(AttributeName = "Tutar")]
        public double Tutar { get; set; }
    }
}