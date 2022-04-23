using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using HandyControl.Controls;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class MainViewModelBase : INotifyPropertyChanged
    {
        public static readonly string xmldatapath = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) + @"\Data.xml";

        public event PropertyChangedEventHandler PropertyChanged;

        public static IEnumerable<int> Yıllar { get; set; }

        public Kategori AramaSeçiliKategori { get; set; }

        public int Boy { get; set; } = 1;

        public ICommand DatabaseSave { get; set; }

        public int En { get; set; } = 1;

        public ICommand FişEkranı { get; set; }

        public ICommand İlaveÜrünEkle { get; set; }

        public Kategori Kategori { get; set; }

        public ICommand KategoriKaydet { get; set; }

        public ICommand KategoriSil { get; set; }

        public ICommand MasaEkSiparişKaydet { get; set; }

        public ICommand MasaKaydet { get; set; }

        public Masalar Masalar { get; set; }

        public ICommand MasaOluştur { get; set; }

        public ICommand MasaSiparişArtır { get; set; }

        public ICommand MasaSiparişEkle { get; set; }

        public ICommand MasaSiparişKaydet { get; set; }

        public ICommand MasaSiparişSil { get; set; }

        public ICommand MasaToplamSiparişDurumuGöster { get; set; }

        public Müşteri Müşteri { get; set; }

        public ICommand MüşteriEkle { get; set; }

        public ICommand MüşteriSeçiliSiparişSil { get; set; }

        public ICommand MüşteriSiparişArttır { get; set; }

        public ICommand MüşteriSiparişEkle { get; set; }

        public ICommand MüşteriSiparişKaydet { get; set; }

        public ICommand MüşteriSiparişTamamla { get; set; }

        public ICommand ÖdemeEkranı { get; set; }

        public ÖdemeViewModel ÖdemeViewModel { get; set; }

        public OptimizedObservableCollection<Masa> ÖnizlemeMasa { get; set; }

        public string SalonAdı { get; set; }

        public bool SalonTabSelected { get; set; } = true;

        public Kategori SeçiliKategori { get; set; }

        public Müşteri SeçiliMüşteri { get; set; }

        public double SeçiliMüşteriSiparişToplamı { get; set; }

        public double SeçiliSalonGünlükSiparişToplamı { get; set; }

        public Siparişler SeçiliSipariş { get; set; }

        public ICommand SeçiliSiparişSil { get; set; }

        public int SeçiliYıl { get; set; } = DateTime.Now.Year;

        public Siparişler Siparişler { get; set; }

        public ICommand SiparişTahsilEt { get; set; }

        public bool TahsilatTabSelected { get; set; }

        public bool TümKayıtlar { get; set; } = true;

        public Ürün Ürün { get; set; }

        public ICommand ÜrünAra { get; set; }

        public string ÜrünAramaMetin { get; set; }

        public ICommand ÜrünKaydet { get; set; }

        public ICommand ÜrünResimYükle { get; set; }

        public Veriler Veriler { get; set; }

        public ICommand WebAdreseGit { get; set; }

        public IEnumerable<Siparişler> YıllarSiparişDurumu { get; set; }

        public static void DepoKontrol(double adet, double eşik)
        {
            if (adet < eşik)
            {
                Growl.Warning($"Dikkat Depoda Bu Üründen {eşik} Adetten Az Ürün Kaldı.");
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}