using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Ürünler")]
    public class Ürünler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Ürün")]
        public ObservableCollection<Ürün> Ürün { get; set; }
    }    }