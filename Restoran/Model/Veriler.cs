using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Veriler")]
    public class Veriler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Masalar")]
        public Masalar Masalar { get; set; }

        [XmlElement(ElementName = "Tahsilatlar")]
        public Tahsilatlar Tahsilatlar { get; set; }

        [XmlElement(ElementName = "Ürünler")]
        public Ürünler Ürünler { get; set; }
    }
}