using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Kategoriler")]
    public class Kategoriler : BaseModel
    {
        [XmlElement(ElementName = "Kategori")]
        public ObservableCollection<Kategori> Kategori { get; set; }
    }
}