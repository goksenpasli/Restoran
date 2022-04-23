using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using PropertyChanged;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Siparişler")]
    public class Siparişler : INotifyPropertyChanged
    {
        private double farkDakika;

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        [DependsOn("TahsilatTarih", "Tarih")]
        public double FarkDakika
        {
            get => TahsilatTarih != DateTime.MinValue ? Math.Round(TahsilatTarih.Subtract(Tarih).TotalMinutes, 2) : 0;

            set => farkDakika = value;
        }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Ödendi")]
        public bool Ödendi { get; set; }

        [XmlElement(ElementName = "Sipariş")]
        public ObservableCollection<Sipariş> Sipariş { get; set; } = new();

        [XmlAttribute(AttributeName = "TahsilatTarih")]
        public DateTime TahsilatTarih { get; set; }

        [XmlAttribute(AttributeName = "Tarih")]
        public DateTime Tarih { get; set; }

        [XmlAttribute(AttributeName = "ToplamTutar")]
        public double ToplamTutar { get; set; }
    }
}