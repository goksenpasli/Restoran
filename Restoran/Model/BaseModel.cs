using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Adet")]
        public int Adet { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Ödendi")]
        public bool Ödendi { get; set; }
    }
}