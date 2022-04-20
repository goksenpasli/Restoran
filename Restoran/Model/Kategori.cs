using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Kategori")]
    public class Kategori
    {
        [XmlAttribute(AttributeName = "Açıklama")]
        public string Açıklama { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }
    }
}