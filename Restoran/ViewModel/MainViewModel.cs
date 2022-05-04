using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using DotLiquid;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Interactivity;
using HandyControl.Tools;
using Microsoft.Win32;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class MainViewModel : MainViewModelBase
    {
        static MainViewModel()
        {
            Yıllar = Enumerable.Range(DateTime.Now.Year - 5, 10);
            ConfigHelper.Instance.SetLang("tr");
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
                Ürünler = ExtensionMethods.Ürünler(),
                Kategoriler = ExtensionMethods.Kategoriler(),
                Müşteriler = ExtensionMethods.Müşteriler()
            };
            VerileriYükle();

            Ürün = new Ürün();
            Kategori = new Kategori();
            Müşteri = new Müşteri();
            Siparişler = new Siparişler();
            Rezervasyonlar = new Rezervasyonlar();
            ÖdemeViewModel = new ÖdemeViewModel();
            DocumentViewModel = new DocumentViewModel();
            RezervasyonViewModel = new RezervasyonViewModel();

            RezervasyonListeleri = new ObservableCollection<Rezervasyonlar>(Veriler.Salonlar.Masalar.SelectMany(z => z.Masa).SelectMany(z => z.Rezervasyonlar));
            Masalar = Veriler?.Salonlar?.Masalar?.FirstOrDefault();
            SeçiliSalonGünlükSiparişToplamı = Masalar?.Masa?.SelectMany(z => z.Siparişler).Where(z => z.Tarih > DateTime.Today && z.Tarih < DateTime.Today.AddDays(1) && z.Ödendi).Sum(z => z.ToplamTutar) ?? 0;

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
                    DepoKontrol(Veriler.DepoÜrünAdeti(ürün.Id), Veriler.DepoÜrünEşikAdeti(ürün.Id));
                }
            }, parameter => Masalar?.SeçiliMasa is not null);

            MasaSiparişArtır = new RelayCommand<object>(parameter =>
            {
                if (parameter is Ürün ürün)
                {
                    if (Siparişler?.Sipariş?.Any(z => z.ÜrünId == ürün.Id) == true)
                    {
                        Siparişler.Sipariş.FirstOrDefault(z => z.ÜrünId == ürün.Id).Adet += ürün.SiparişAdet;
                        DepoKontrol(Veriler.DepoÜrünAdeti(ürün.Id), Veriler.DepoÜrünEşikAdeti(ürün.Id));
                    }
                    else
                    {
                        MasaSiparişEkle.Execute(ürün);
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
                        Growl.Success("Sipariş Silindi.");
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
                    siparişler.TahsilatTarih = DateTime.Now;
                    Masalar.SeçiliMasa.Dolu = false;
                    if (Masalar.SeçiliMasa.Rezervasyonlar.All(z => z.RezervasyonTarihi <= DateTime.Now))
                    {
                        Masalar.SeçiliMasa.Rezerve = false;
                    }
                    ExtensionMethods.ÜrünAdetDüşümüYap(siparişler.Sipariş, Veriler.Ürünler.Ürün);

                    DatabaseSave.Execute(null);
                    Growl.Success("Tahsil Edildi.");
                    OnPropertyChanged(nameof(Masalar));
                    ControlCommands.Close.Execute(null, null);
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
                    Growl.Success("Masa Düzeni Oluşturuldu.");

                    ÖnizlemeMasa.Clear();
                    En = 1;
                    Boy = 1;
                    SalonAdı = null;
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
                        Favori = Ürün.Favori,
                        KategoriId = SeçiliKategori.Id,
                        Barkod = Ürün.Barkod
                    };

                    Veriler?.Ürünler?.Ürün?.Add(ürün);
                    DatabaseSave.Execute(null);
                    Ürün.Resim = null;
                    Growl.Success("Ürün Kaydedildi.");
                }
            }, parameter => !string.IsNullOrWhiteSpace(Ürün?.Açıklama) && SeçiliKategori is not null);

            KategoriKaydet = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Kategori Kaydedilsin mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    Kategori kategori = new()
                    {
                        Açıklama = Kategori.Açıklama,
                        Id = ExtensionMethods.RandomNumber()
                    };

                    Veriler?.Kategoriler?.Kategori?.Add(kategori);
                    DatabaseSave.Execute(null);
                    Kategori.Açıklama = null;
                    Growl.Success("Kategori Kaydedildi.");
                }
            }, parameter => !string.IsNullOrWhiteSpace(Kategori?.Açıklama));

            KategoriSil = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Kategori Silinsin mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    _ = Veriler?.Kategoriler?.Kategori?.Remove(parameter as Kategori);
                    DatabaseSave.Execute(null);
                    Growl.Success("Kategori Silindi.");
                }
            }, parameter => true);

            ÜrünResimYükle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    Ürün.Resim = openFileDialog.FileName.ResimYükle(64, 64);
                }
            }, parameter => true);

            ÜrünResimGüncelle = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new() { Multiselect = false, Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.jpg;*.jpeg;*.tif;*.tiff;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    (parameter as Ürün).Resim = openFileDialog.FileName.ResimYükle(64, 64);
                }
            }, parameter => true);

            SiparişTaşı = new RelayCommand<object>(parameter =>
            {
                if (!TaşınacakMasa.Dolu && !TaşınacakMasa.Gizli)
                {
                    TaşınacakMasa.Siparişler.Add(SeçiliSipariş);
                    TaşınacakMasa.Dolu = true;
                    _ = Masalar?.SeçiliMasa?.Siparişler?.Remove(SeçiliSipariş);
                    Masalar.SeçiliMasa.Dolu = false;
                    DatabaseSave.Execute(null);
                    TaşınacakMasa = null;
                }
                else
                {
                    Growl.Warning("Taşınacak Masa Dolu Veya Gizli Olmamalıdır. Başka Masa Seçin.");
                }
            }, parameter => TaşınacakMasa is not null);

            FişEkranı = new RelayCommand<object>(parameter =>
            {
                if (File.Exists("Report\\report.lqd"))
                {
                    Hash fiş = Hash.FromAnonymousObject(new
                    {
                        Siparişler = SeçiliSipariş?.Sipariş.Select(sipariş => new Ürün() { Fiyat = Veriler.Ürünler.Ürün.FirstOrDefault(ürün => ürün.Id == sipariş.ÜrünId).Fiyat, Adet = sipariş.Adet, Açıklama = Veriler.Ürünler.Ürün.FirstOrDefault(z => z.Id == sipariş.ÜrünId).Açıklama }),
                        Toplam = SeçiliSipariş?.ToplamTutar,
                        MasaNo = Masalar.SeçiliMasa.No
                    });
                    string template = fiş.GenerateTemplate("Report\\report.lqd");
                    FlowDocument fd = (FlowDocument)XamlReader.Parse(template);
                    DocumentViewModel.Document = fd.WriteXPS();
                    DocumentViewModel.Başlık = "FİŞ";
                    _ = Dialog.Show(DocumentViewModel);
                }
            }, parameter => SeçiliSipariş is not null);

            GünlükRaporEkranı = new RelayCommand<object>(parameter =>
            {
                if (File.Exists("Report\\report.lqd"))
                {
                    IEnumerable<Siparişler> data = Masalar?.Masa?.SelectMany(z => z.Siparişler).Where(z => z.Tarih > RaporSeçiliGün && z.Tarih < RaporSeçiliGün.AddDays(1));
                    if (data != null)
                    {
                        Hash günlükrapor = Hash.FromAnonymousObject(new
                        {
                            Siparişler = data.OrderByDescending(z => z.Tarih),
                            GenelToplam = data.Sum(z => z.ToplamTutar)
                        });
                        string template = günlükrapor.GenerateTemplate("Report\\siparişler.lqd");
                        FlowDocument fd = (FlowDocument)XamlReader.Parse(template);
                        DocumentViewModel.Document = fd.WriteXPS();
                        DocumentViewModel.Başlık = "RAPOR";
                        _ = Dialog.Show(DocumentViewModel);
                    }
                }
            }, parameter => true);

            ÖdemeEkranı = new RelayCommand<object>(parameter =>
            {
                SeçiliSipariş.ToplamTutar = SeçiliSipariş?.Sipariş?.SiparişToplamları() ?? 0;
                ÖdemeViewModel.GeriVerilecek = 0;
                ÖdemeViewModel.Siparişler = null;
                ÖdemeViewModel.Siparişler = SeçiliSipariş;
                _ = Dialog.Show(ÖdemeViewModel);
            }, parameter => SeçiliSipariş is not null);

            RezervasyonEkranı = new RelayCommand<object>(parameter =>
            {
                RezervasyonViewModel.SeçiliMasa = parameter as Masa;
                _ = Dialog.Show(RezervasyonViewModel);
            }, parameter => true);

            MüşteriSiparişEkle = new RelayCommand<object>(parameter =>
            {
                if (parameter is Ürün ürün)
                {
                    Sipariş sipariş = new()
                    {
                        ÜrünId = ürün.Id,
                        Adet = 1,
                    };

                    SeçiliMüşteri.Sipariş.Add(sipariş);
                    DepoKontrol(Veriler.DepoÜrünAdeti(ürün.Id), Veriler.DepoÜrünEşikAdeti(ürün.Id));
                }
            }, parameter => !string.IsNullOrWhiteSpace(Müşteri?.Ad) && !string.IsNullOrWhiteSpace(Müşteri?.Soyad) && !string.IsNullOrWhiteSpace(Müşteri?.Adres));

            MüşteriEkle = new RelayCommand<object>(parameter =>
            {
                Müşteri müşteri = new()
                {
                    Ad = Müşteri.Ad,
                    Soyad = Müşteri.Soyad,
                    Adres = Müşteri.Adres,
                    Id = ExtensionMethods.RandomNumber()
                };

                Veriler.Müşteriler.Müşteri.Add(müşteri);
                DatabaseSave.Execute(null);
                ResetMüşteri();
            }, parameter => !string.IsNullOrWhiteSpace(Müşteri?.Ad) && !string.IsNullOrWhiteSpace(Müşteri?.Soyad) && !string.IsNullOrWhiteSpace(Müşteri?.Adres));

            MüşteriSiparişArttır = new RelayCommand<object>(parameter =>
            {
                if (parameter is Ürün ürün)
                {
                    if (SeçiliMüşteri.Sipariş?.Any(z => z.ÜrünId == ürün.Id) == true)
                    {
                        SeçiliMüşteri.Sipariş.FirstOrDefault(z => z.ÜrünId == ürün.Id).Adet++;
                        DepoKontrol(Veriler.DepoÜrünAdeti(ürün.Id), Veriler.DepoÜrünEşikAdeti(ürün.Id));
                    }
                    else
                    {
                        MüşteriSiparişEkle.Execute(ürün);
                    }
                    SeçiliMüşteriSiparişToplamı = SeçiliMüşteri.Sipariş.SiparişToplamları();
                }
            }, parameter => SeçiliMüşteri is not null);

            MüşteriSeçiliSiparişSil = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Seçili Sipariş Silinsin mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes && parameter is Sipariş sipariş)
                {
                    _ = SeçiliMüşteri?.Sipariş?.Remove(sipariş);
                    SeçiliMüşteriSiparişToplamı = SeçiliMüşteri.Sipariş.SiparişToplamları();
                    DatabaseSave.Execute(null);
                }
            }, parameter => SeçiliMüşteri?.Ödendi == false);

            MüşteriSiparişTamamla = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Müşterinin Siparişi Tahsil Edildi mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    SeçiliMüşteri.Ödendi = true;
                    SeçiliMüşteri.Tarih = DateTime.Now;
                    SeçiliMüşteri.ToplamTutar = SeçiliMüşteriSiparişToplamı;
                    DatabaseSave.Execute(null);
                    Growl.Success("Sipariş Kaydedildi.");
                    SeçiliMüşteri = null;
                }
            }, parameter => SeçiliMüşteri?.Sipariş?.Any() == true && SeçiliMüşteri?.Ödendi == false);

            ÜrünAra = new RelayCommand<object>(parameter => MainWindow.cvsürün.Filter += (s, e) => e.Accepted &= (e.Item as Ürün)?.Açıklama.Contains(ÜrünAramaMetin) == true || (e.Item as Ürün)?.Barkod == ÜrünAramaMetin, parameter => true);

            MüşteriAra = new RelayCommand<object>(parameter => MainWindow.cvsmüşteri.Filter += (s, e) => e.Accepted &= (e.Item as Müşteri)?.Ad.Contains(MüşteriAramaMetin) == true, parameter => true);

            WebAdreseGit = new RelayCommand<object>(parameter => Process.Start(parameter as string), parameter => true);

            #endregion Commands
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
                IEnumerable<Rezervasyonlar> Rezervasyonlar = Masalar?.Masa?.SelectMany(z => z.Rezervasyonlar).Where(z => z.RezervasyonTarihi > DateTime.Today && z.RezervasyonTarihi < DateTime.Today.AddDays(1));
                SeçiliSalonGünlükSiparişToplamı = Masalar?.Masa?.SelectMany(z => z.Siparişler).Where(z => z.Tarih > DateTime.Today && z.Tarih < DateTime.Today.AddDays(1) && z.Ödendi).Sum(z => z.ToplamTutar) ?? 0;
            }
            if (e.PropertyName is "AramaSeçiliKategori")
            {
                if (AramaSeçiliKategori is null)
                {
                    MainWindow.cvsürün.Filter += (s, e) => e.Accepted = true;
                    return;
                }
                MainWindow.cvsürün.Filter += (s, e) => e.Accepted &= (e.Item as Ürün)?.KategoriId == AramaSeçiliKategori?.Id;
            }
            if (e.PropertyName is "SeçiliMüşteri" && SeçiliMüşteri is not null)
            {
                SeçiliMüşteriSiparişToplamı = SeçiliMüşteri.Sipariş.SiparişToplamları();
            }
        }

        private void ResetMüşteri()
        {
            Müşteri.Ad = null;
            Müşteri.Soyad = null;
            Müşteri.Adres = null;
        }

        private void VerileriYükle()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Veriler.Ürünler.Ürün = ExtensionMethods.ÜrünleriYükle();
                Veriler.Salonlar.Masalar = ExtensionMethods.MasalarıYükle();
                Veriler.Kategoriler.Kategori = ExtensionMethods.KategorileriYükle();
                Veriler.Müşteriler.Müşteri = ExtensionMethods.MüşterileriYükle();
            }
        }
    }
}