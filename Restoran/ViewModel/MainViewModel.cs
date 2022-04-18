using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using DotLiquid;
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using Restoran.Model;
using dotTemplate = DotLiquid.Template;

namespace Restoran.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static readonly string xmldatapath = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) + @"\Data.xml";

        static MainViewModel()
        {
            Yıllar = Enumerable.Range(DateTime.Now.Year - 5, 10);
        }

        public MainViewModel()
        {
            if (!File.Exists(xmldatapath))
            {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(xmldatapath));
            }

            Veriler = new Veriler
            {
                Salonlar = ExtensionMethods.Salonlar(),
                Ürünler = ExtensionMethods.Ürünler()
            };
            Ürün = new Ürün();
            Siparişler = new();

            VerileriYükle();
            Masalar = Veriler.Salonlar.Masalar.FirstOrDefault();
            SeçiliSalonGünlükSiparişToplamı = Masalar?.Masa?.SelectMany(z => z.Siparişler).Where(z => z.Tarih > DateTime.Today && z.Tarih < DateTime.Today.AddDays(1)).Sum(z => z.ToplamTutar) ?? 0;

            PropertyChanged += MainViewModel_PropertyChanged;

            #region Commands

            DatabaseSave = new RelayCommand<object>(parameter =>
                {
                    try
                    {
                        Veriler.Serialize();
                    }
                    catch (Exception Ex)
                    {
                        _ = HandyControl.Controls.MessageBox.Show(Ex.Message);
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
                    double adet = Veriler.DepoÜrünAdeti(ürün.Id);
                    double eşik = Veriler.DepoÜrünEşikAdeti(ürün.Id);
                    if (adet < eşik)
                    {
                        Growl.Warning($"Dikkat Depoda Bu Üründen {eşik} Adetten Az Ürün Kaldı.");
                    }
                }
            }, parameter => Masalar?.SeçiliMasa is not null);

            SeçiliSiparişSil = new RelayCommand<object>(parameter =>
            {
                if (parameter is Sipariş sipariş)
                {
                    _ = Siparişler.Sipariş.Remove(sipariş);
                }
            }, parameter => Masalar?.SeçiliMasa is not null);

            MasaSiparişSil = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Seçili Sipariş Silinsin mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    if (parameter is Siparişler siparişler)
                    {
                        _ = Masalar?.SeçiliMasa?.Siparişler?.Remove(siparişler);
                        Masalar.SeçiliMasa.Dolu = false;
                        DatabaseSave.Execute(null);
                    }
                }
            }, parameter => Masalar?.SeçiliMasa is not null);

            MasaToplamSiparişDurumuGöster = new RelayCommand<object>(parameter =>
            {
                if (parameter is Masalar masalar)
                {
                    foreach (Masa item in masalar.Masa.ToList())
                    {
                        item.MasaToplamSiparişDurumuGöster = masalar.MasaDurumunuGöster;
                    }
                }
            }, parameter => true);

            MasaSiparişKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Sipariş Kaydını Onaylıyor musun?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    Masalar?.SeçiliMasa?.Siparişler?.Add(Siparişler);
                    Masalar.SeçiliMasa.Dolu = true;
                    DatabaseSave.Execute(null);
                    Siparişler = new();
                    Growl.Success("Sipariş Kaydedildi.");
                }
            }, parameter => Masalar?.SeçiliMasa?.Dolu == false && Siparişler?.Sipariş?.Any() == true);

            MasaEkSiparişKaydet = new RelayCommand<object>(parameter =>
            {
                if (parameter is Siparişler siparişler && HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "İlave Sipariş Kaydını Onaylıyor musun?", Caption = "Kaydet", Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    foreach (Sipariş item in Siparişler.Sipariş)
                    {
                        siparişler.Sipariş.Add(item);
                    }
                    DatabaseSave.Execute(null);
                    Siparişler = new();
                    Growl.Success("İlave Sipariş Kaydedildi.");
                }
            }, parameter => Masalar?.SeçiliMasa?.Dolu == true && Siparişler?.Sipariş?.Any() == true);

            SiparişTahsilEt = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Tahsilatı Onaylıyor musun?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes && parameter is Siparişler siparişler)
                {
                    siparişler.Ödendi = true;
                    siparişler.ToplamTutar = siparişler.Sipariş.SiparişToplamları();
                    siparişler.TahsilatTarih = DateTime.Now;
                    Masalar.SeçiliMasa.Dolu = false;
                    ExtensionMethods.ÜrünAdetDüşümüYap(siparişler.Sipariş, Veriler.Ürünler.Ürün);

                    DatabaseSave.Execute(null);
                    Growl.Success("Tahsil Edildi.");
                    OnPropertyChanged(nameof(Masalar));
                }
            }, parameter => parameter is Siparişler sipariş && sipariş?.Ödendi == false);

            MasaKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Düzen Kaydedilsin mi? Bu Düzen Daha Değiştirilmeyecektir.", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    Masalar masalar = new()
                    {
                        En = En,
                        Boy = Boy,
                        Id = ExtensionMethods.RandomNumber(),
                        SalonAdı = SalonAdı
                    };
                    foreach (Masa item in ÖnizlemeMasa)
                    {
                        masalar.Masa.Add(item);
                    }

                    Veriler?.Salonlar?.Masalar?.Add(masalar);
                    DatabaseSave.Execute(null);
                    ÖnizlemeMasa.Clear();
                    Growl.Success("Masa Düzeni Oluşturuldu.");
                }
            }, parameter => ÖnizlemeMasa?.Count(z => !z.Gizli) > 0 && !string.IsNullOrWhiteSpace(SalonAdı));

            ÜrünKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Ürün Kaydedilsin mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    Ürün ürün = new()
                    {
                        Açıklama = Ürün.Açıklama,
                        Fiyat = Ürün.Fiyat,
                        Adet = Ürün.Adet,
                        Mevcut = true,
                        Id = ExtensionMethods.RandomNumber(),
                        Resim = Ürün.Resim,
                        UyarıAdet = Ürün.UyarıAdet,
                        Favori = Ürün.Favori
                    };

                    Veriler?.Ürünler?.Ürün?.Add(ürün);
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

            FişEkranı = new RelayCommand<object>(parameter =>
            {
                if (File.Exists("Report\\report.lqd"))
                {
                    string template = GenerateTemplate("Report\\report.lqd");
                    FişView fiş = new();
                    FlowDocument fd = (FlowDocument)XamlReader.Parse(template);
                    fiş.DocViewer.Document = fd.WriteXPS();
                    _ = Dialog.Show(fiş);
                }
            }, parameter => SeçiliSipariş is not null);

            WebAdreseGit = new RelayCommand<object>(parameter => Process.Start(parameter as string), parameter => true);

            #endregion Commands
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ICommand DatabaseSave { get; set; }

        public static IEnumerable<int> Yıllar { get; set; }

        public int Boy { get; set; } = 1;

        public int En { get; set; } = 1;

        public ICommand FişEkranı { get; }

        public ICommand MasaEkSiparişKaydet { get; }

        public ICommand MasaKaydet { get; }

        public Masalar Masalar { get; set; }

        public ICommand MasaOluştur { get; }

        public ICommand MasaSiparişEkle { get; }

        public ICommand MasaSiparişKaydet { get; }

        public ICommand MasaSiparişSil { get; }

        public ICommand MasaToplamSiparişDurumuGöster { get; }

        public OptimizedObservableCollection<Masa> ÖnizlemeMasa { get; set; }

        public string SalonAdı { get; set; }

        public bool SalonTabSelected { get; set; } = true;

        public double SeçiliSalonGünlükSiparişToplamı { get; set; }

        public Siparişler SeçiliSipariş { get; set; }

        public ICommand SeçiliSiparişSil { get; }

        public int SeçiliYıl { get; set; } = DateTime.Now.Year;

        public Siparişler Siparişler { get; set; }

        public ICommand SiparişTahsilEt { get; }

        public bool TahsilatTabSelected { get; set; }

        public bool TümKayıtlar { get; set; } = true;

        public Ürün Ürün { get; set; }

        public ICommand ÜrünKaydet { get; }

        public ICommand ÜrünResimYükle { get; }

        public Veriler Veriler { get; set; }

        public ICommand WebAdreseGit { get; }

        public IEnumerable<Siparişler> YıllarSiparişDurumu { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Hash CreateDocumentContext()
        {
            return Hash.FromAnonymousObject(new
            {
                Siparişler = SeçiliSipariş?.Sipariş.Select(sipariş => new Ürün() { Fiyat = Veriler.Ürünler.Ürün.FirstOrDefault(ürün => ürün.Id == sipariş.ÜrünId).Fiyat, Adet = sipariş.Adet, Açıklama = Veriler.Ürünler.Ürün.FirstOrDefault(z => z.Id == sipariş.ÜrünId).Açıklama }),
                Toplam = SeçiliSipariş?.Sipariş.SiparişToplamları(),
                MasaNo = Masalar.SeçiliMasa.No
            });
        }

        private string GenerateTemplate(string reportpath)
        {
            using FileStream stream = new(reportpath, FileMode.Open);
            using StreamReader reader = new(stream);
            dotTemplate template = dotTemplate.Parse(reader.ReadToEnd());
            Hash docContext = CreateDocumentContext();
            return template.Render(docContext);
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "En" or "Boy")
            {
                MasaOluştur.Execute(null);
            }
            if (e.PropertyName is "SeçiliYıl" || (e.PropertyName is "TahsilatTabSelected" && TahsilatTabSelected))
            {
                YıllarSiparişDurumu = Veriler.SiparişDurumuVerileriniAl(SeçiliYıl);
            }
            if (e.PropertyName is "SalonTabSelected" && SalonTabSelected)
            {
                VerileriYükle();
            }
            if (e.PropertyName is "TümKayıtlar")
            {
                if (TümKayıtlar)
                {
                    MainWindow.cvs.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                MainWindow.cvs.Filter += (s, e) => e.Accepted = (e.Item as Siparişler)?.Tarih > DateTime.Today;
            }
            if (e.PropertyName is "Masalar")
            {
                SeçiliSalonGünlükSiparişToplamı = Masalar?.Masa?.SelectMany(z => z.Siparişler).Where(z => z.Tarih > DateTime.Today && z.Tarih < DateTime.Today.AddDays(1)).Sum(z => z.ToplamTutar) ?? 0;
            }
        }

        private void VerileriYükle()
        {
            Veriler.Ürünler.Ürün = ExtensionMethods.ÜrünleriYükle();
            Veriler.Salonlar.Masalar = ExtensionMethods.MasalarıYükle();
        }
    }
}