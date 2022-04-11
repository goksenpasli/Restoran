using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Siparişler")]
    public class Siparişler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Ödendi")]
        public bool Ödendi { get; set; }

        [XmlElement(ElementName = "Sipariş")]
        public ObservableCollection<Sipariş> Sipariş { get; set; } = new();

        [XmlAttribute(AttributeName = "Tarih")]
        public DateTime Tarih { get; set; }

        [XmlAttribute(AttributeName = "ToplamTutar")]
        public double ToplamTutar { get; set; }
    }
}