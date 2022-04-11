using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Masalar")]
    public class Masalar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Boy")]
        public int Boy { get; set; }

        [XmlAttribute(AttributeName = "En")]
        public int En { get; set; }

        [XmlElement(ElementName = "Masa")]
        public ObservableCollection<Masa> Masa { get; set; }
    }
}