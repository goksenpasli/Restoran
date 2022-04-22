using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Müşteriler")]
    public class Müşteriler : BaseModel
    {
        [XmlElement(ElementName = "Müşteri")]
        public ObservableCollection<Müşteri> Müşteri { get; set; }
    }
}