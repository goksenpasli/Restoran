using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using DotLiquid;
using HandyControl.Controls;
using Restoran.ViewModel;

namespace Restoran.Model
{
    [XmlRoot(ElementName = "Ürün")]
    public class Ürün : INotifyPropertyChanged, ILiquidizable
    {
        public Ürün()
        {
            İlaveÜrünEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Ürün ürün)
                {
                    ürün.Adet += ürün.İlaveÜrünAdeti;
                    MainViewModel.DatabaseSave.Execute(null);
                    ürün.İlaveÜrünAdeti = 1;
                }
            }, parameter => true);
            PropertyChanged += Ürün_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute(AttributeName = "Açıklama")]
        public string Açıklama { get; set; }

        [XmlAttribute(AttributeName = "Adet")]
        public int Adet { get; set; }

        [XmlAttribute(AttributeName = "Favori")]
        public bool Favori { get; set; }

        [XmlAttribute(AttributeName = "Fiyat")]
        public double Fiyat { get; set; } = 1;

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlIgnore]
        public int İlaveÜrünAdeti { get; set; } = 1;

        public ICommand İlaveÜrünEkle { get; }

        [XmlAttribute(AttributeName = "Mevcut")]
        public bool Mevcut { get; set; }

        [XmlAttribute(AttributeName = "Resim")]
        public string Resim { get; set; }

        [XmlIgnore]
        public int SiparişAdet { get; set; } = 1;

        [XmlAttribute(AttributeName = "UyarıAdet")]
        public int UyarıAdet { get; set; } = 50;

        public object ToLiquid()
        {
            return new { Adet, Açıklama, Fiyat, Id };
        }

        private void Ürün_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SiparişAdet")
            {
                if (SiparişAdet > ExtensionMethods.ÜrünleriYükle().FirstOrDefault(z => z.Id == Id).Adet)
                {
                    Growl.Warning("Dikkat Depoda Bu Kadar Ürün Yok.");
                }
            }
        }
    }
}