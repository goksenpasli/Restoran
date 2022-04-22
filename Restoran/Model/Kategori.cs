using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Kategori")]
    public class Kategori : BaseModel
    {
        [XmlAttribute(AttributeName = "Açıklama")]
        public string Açıklama { get; set; }
    }
}