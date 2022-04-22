using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Salonlar")]
    public class Salonlar : BaseModel
    {
        [XmlElement(ElementName = "Masalar")]
        public ObservableCollection<Masalar> Masalar { get; set; } = new();
    }
}