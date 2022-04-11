using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Tahsilatlar")]
    public class Tahsilatlar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Tahsilat")]
        public ObservableCollection<Tahsilat> Tahsilat { get; set; }
    }
}