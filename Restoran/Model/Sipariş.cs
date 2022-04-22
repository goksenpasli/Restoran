using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Sipariş")]
    public class Sipariş : BaseModel
    {
        [XmlAttribute(AttributeName = "ÜrünId")]
        public int ÜrünId { get; set; }
    }
}