using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Salonlar")]
    public class Salonlar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement(ElementName = "Masalar")]
        public ObservableCollection<Masalar> Masalar { get; set; } = new();
    }
}