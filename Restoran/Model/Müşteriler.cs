using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Müşteriler")]
    public class Müşteriler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Müşteri")]
        public ObservableCollection<Müşteri> Müşteri { get; set; }
    }
}