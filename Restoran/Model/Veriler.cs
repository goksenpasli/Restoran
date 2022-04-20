using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Veriler")]
    public class Veriler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Salonlar")]
        public Salonlar Salonlar { get; set; }

        [XmlElement(ElementName = "Tahsilatlar")]
        public Tahsilatlar Tahsilatlar { get; set; }

        [XmlElement(ElementName = "Ürünler")]
        public Ürünler Ürünler { get; set; }  
        
        [XmlElement(ElementName = "Kategoriler")]
        public Kategoriler Kategoriler { get; set; }
    }
}