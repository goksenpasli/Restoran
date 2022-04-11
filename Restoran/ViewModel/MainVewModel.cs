using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class MainVewModel : INotifyPropertyChanged
    {
        public static readonly string xmldatapath = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) + @"\Data.xml";

        public MainVewModel()
        {
            if (!File.Exists(xmldatapath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(xmldatapath));
            }

            Veriler = new Veriler
            {
                Masalar = ExtensionMethods.Masalar(),
                Ürünler = ExtensionMethods.Ürünler()
            };

            Veriler.Ürünler.Ürün = ExtensionMethods.ÜrünleriYükle();
            Veriler.Masalar.Masa = ExtensionMethods.MasalarıYükle();

            Ürün = new Ürün();

            DatabaseSave = new RelayCommand<object>(parameter =>
            {
                try
                {
                    Veriler.Serialize();
                }
                catch (Exception Ex)
                {
                    HandyControl.Controls.MessageBox.Show(Ex.Message);
                }
            }, parameter => File.Exists(xmldatapath));

            MasaOluştur = new RelayCommand<object>(parameter =>
            {
                ÖnizlemeMasa = new OptimizedObservableCollection<Masa>();
                for (int i = 0; i < En * Boy; i++)
                {
                    ÖnizlemeMasa.Add(new Masa() { Id = ExtensionMethods.RandomNumber(), Renk = "#000000", Dolu = false, Gizli = false, Kapasite = 1, No = i + 1, });
                }
            }, parameter => true);

            Siparişler = new();
            MasaSiparişEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Ürün ürün)
                {
                    Sipariş sipariş = new()
                    {
                        ÜrünId = ürün.Id,
                        Adet = ürün.SiparişAdet,
                    };

                    Siparişler.Tarih = DateTime.Now;
                    Siparişler.Id = ExtensionMethods.RandomNumber();
                    Siparişler.Sipariş.Add(sipariş);
                    ürün.SiparişAdet = 1;
                }
            }, parameter => SeçiliMasa is not null);

            MasaSiparişSil = new RelayCommand<object>(parameter =>
            {
                if (parameter is Sipariş sipariş)
                {
                    Siparişler.Sipariş.Remove(sipariş);
                }
            }, parameter => SeçiliMasa is not null);

            MasaSiparişKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Sipariş Kaydını Onaylıyor musun?", Caption = "Kaydet", Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    SeçiliMasa.Siparişler.Add(Siparişler);
                    SeçiliMasa.Dolu = true;
                    DatabaseSave.Execute(null);
                    Siparişler = new();
                    Growl.Success("Sipariş Kaydedildi.");
                }
            }, parameter => SeçiliMasa?.Dolu == false && Siparişler?.Sipariş?.Any() == true);

            SiparişTahsilEt = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Tahsilatı Onaylıyor musun?", Caption = "Kaydet", Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    if (parameter is Siparişler siparişler)
                    {
                        siparişler.Ödendi = true;
                        siparişler.ToplamTutar = siparişler.Sipariş.SiparişToplamları();
                        SeçiliMasa.Dolu = false;
                        DatabaseSave.Execute(null);
                        Growl.Success("Tahsil Edildi.");
                    }
                }
            }, parameter => parameter is Siparişler sipariş && sipariş?.Ödendi == false);

            MasaKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Düzen Kaydedilsin mi? Bu Düzen Daha Değiştirilmeyecektir.", Caption = "Kaydet", Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    Veriler.Masalar.En = En;
                    Veriler.Masalar.Boy = Boy;
                    foreach (Masa item in ÖnizlemeMasa)
                    {
                        Veriler.Masalar.Masa.Add(item);
                    }
                    DatabaseSave.Execute(null);
                    ÖnizlemeMasa.Clear();
                    Growl.Success("Masa Düzeni Oluşturuldu.");
                }
            }, parameter => ÖnizlemeMasa?.Count(z => !z.Gizli) > 0 && Veriler?.Masalar?.Masa.Count <= 0);

            ÜrünKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Ürün Kaydedilsin mi?", Caption = "Kaydet", Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    Ürün ürün = new()
                    {
                        Açıklama = Ürün.Açıklama,
                        Fiyat = Ürün.Fiyat,
                        Adet = Ürün.Adet,
                        Mevcut = true,
                        Id = ExtensionMethods.RandomNumber(),
                        Resim = Ürün.Resim
                    };

                    Veriler.Ürünler.Ürün.Add(ürün);
                    DatabaseSave.Execute(null);
                    Ürün.Resim = null;
                    Growl.Success("Ürün Kaydedildi.");
                }
            }, parameter => !string.IsNullOrWhiteSpace(Ürün?.Açıklama));

            ÜrünResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    Ürün.Resim = openFileDialog.FileName.ResimYükle(64, 64);
                }
            }, parameter => true);

            WebAdreseGit = new RelayCommand<object>(parameter => Process.Start(parameter as string), parameter => true);

            PropertyChanged += MainVewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Boy { get; set; } = 1;

        public RelayCommand<object> DatabaseSave { get; }

        public int En { get; set; } = 1;

        public RelayCommand<object> MasaKaydet { get; }

        public RelayCommand<object> MasaOluştur { get; }

        public RelayCommand<object> MasaSiparişEkle { get; }

        public RelayCommand<object> MasaSiparişKaydet { get; }

        public RelayCommand<object> MasaSiparişSil { get; }

        public OptimizedObservableCollection<Masa> ÖnizlemeMasa { get; set; }

        public Masa SeçiliMasa { get; set; }

        public Siparişler Siparişler { get; set; }

        public RelayCommand<object> SiparişTahsilEt { get; }

        public Ürün Ürün { get; set; }

        public RelayCommand<object> ÜrünKaydet { get; }

        public RelayCommand<object> ÜrünResimYükle { get; }

        public Veriler Veriler { get; set; }

        public RelayCommand<object> WebAdreseGit { get; }

        private void MainVewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "En" or "Boy")
            {
                MasaOluştur.Execute(null);
            }
        }
    }
}