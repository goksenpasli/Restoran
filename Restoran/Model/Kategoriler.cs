using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Kategoriler")]
    public class Kategoriler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Kategori")]
        public ObservableCollection<Kategori> Kategori { get; set; }
    }
}