using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Sipariş")]
    public class Sipariş : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Adet")]
        public int Adet { get; set; }

        public ÖdemeŞekli ÖdemeŞekli { get; set; }

        [XmlAttribute(AttributeName = "ÜrünId")]
        public int ÜrünId { get; set; }
    }
}