using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Ürünler")]
    public class Ürünler : BaseModel
    {
        [XmlElement(ElementName = "Ürün")]
        public ObservableCollection<Ürün> Ürün { get; set; }
    }
}