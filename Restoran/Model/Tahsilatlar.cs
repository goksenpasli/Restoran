using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Tahsilatlar")]
    public class Tahsilatlar : BaseModel
    {
        [XmlElement(ElementName = "Tahsilat")]
        public ObservableCollection<Tahsilat> Tahsilat { get; set; }
    }
}