using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Tahsilat")]
    public class Tahsilat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "SiparişlerId")]
        public int SiparişlerId { get; set; }

        [XmlAttribute(AttributeName = "Tutar")]
        public double Tutar { get; set; }
    }
}