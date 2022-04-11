using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Ürün")]
    public class Ürün : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Açıklama")]
        public string Açıklama { get; set; }

        [XmlAttribute(AttributeName = "Adet")]
        public int Adet { get; set; } = 1;

        [XmlAttribute(AttributeName = "Fiyat")]
        public double Fiyat { get; set; } = 1;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Mevcut")]
        public bool Mevcut { get; set; }

        [XmlAttribute(AttributeName = "Resim")]
        public string Resim { get; set; }

        [XmlIgnore]
        public int SiparişAdet { get; set; } = 1;
    }
}