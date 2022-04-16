using System.ComponentModel;
using System.Xml.Serialization;
using DotLiquid;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Sipariş")]
    public class Sipariş : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Adet")]
        public int Adet { get; set; }

        [XmlAttribute(AttributeName = "ÜrünId")]
        public int ÜrünId { get; set; }

        //public object ToLiquid()
        //{
        //    return new { Adet, ÜrünId };
        //}
    }
}