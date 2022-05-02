using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Rezervasyonlar")]
    public class Rezervasyonlar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Ad")]
        public string Ad { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }   
        
        [XmlAttribute(AttributeName = "MasaId")]
        public int MasaId { get; set; }

        [XmlAttribute(AttributeName = "İptal")]
        public bool İptal { get; set; }

        [XmlAttribute(AttributeName = "RezervasyonTarihi")]
        public DateTime RezervasyonTarihi { get; set; } = DateTime.Today;

        [XmlAttribute(AttributeName = "Soyad")]
        public string Soyad { get; set; }

        [XmlAttribute(AttributeName = "Telefon")]
        public string Telefon { get; set; }
    }
}