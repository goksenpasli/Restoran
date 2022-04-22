using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Data;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class MüşteriSatışViewModel : MainViewModel
    {
        public MüşteriSatışViewModel()
        {
            Müşteri = new Müşteri();

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
                }
            }, parameter => SeçiliMüşteri is not null);

            MüşteriSeçiliSiparişSil = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Seçili Sipariş Silinsin mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    if (parameter is Sipariş sipariş)
                    {
                        _ = SeçiliMüşteri?.Sipariş?.Remove(sipariş);
                        DatabaseSave.Execute(null);
                    }
                }
            }, parameter => SeçiliMüşteri?.Ödendi == false);

            MüşteriSiparişTamamla = new RelayCommand<object>(parameter =>
            {
                if (HandyControl.Controls.MessageBox.Show(new MessageBoxInfo { Message = "Müşterinin Siparişi Tahsil Edildi mi?", Caption = App.Current.MainWindow.Title, Button = MessageBoxButton.YesNo, IconBrushKey = ResourceToken.AccentBrush, IconKey = ResourceToken.AskGeometry, StyleKey = "MessageBoxCustom" }) == MessageBoxResult.Yes)
                {
                    SeçiliMüşteri.Ödendi = true;
                    DatabaseSave.Execute(null);
                    Growl.Success("Sipariş Kaydedildi.");
                    SeçiliMüşteri = null;
                }
            }, parameter => SeçiliMüşteri?.Sipariş?.Any() == true && SeçiliMüşteri?.Ödendi == false);
        }

        public new event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        private void ResetMüşteri()
        {
            Müşteri.Ad = null;
            Müşteri.Soyad = null;
            Müşteri.Adres = null;
        }
    }
}