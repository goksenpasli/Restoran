using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class ÖdemeViewModel : INotifyPropertyChanged
    {
        public ÖdemeViewModel()
        {
            İndirimUygula = new RelayCommand<object>(parameter =>
            {
                double indirim = (100 - double.Parse(parameter.ToString())) / 100;
                SeçiliSipariş.İndirimliFiyat = SeçiliSipariş.FiyatHesapla() * indirim;
            }, parameter => true);

            GeriVerilecekTutarıHesapla = new RelayCommand<object>(parameter =>
            {
                double para = double.Parse(parameter.ToString());
                GeriVerilecek = para - Siparişler.ToplamTutar;
            }, parameter => true);

            PropertyChanged += ÖdemeViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double GeriVerilecek { get; set; }

        public ICommand GeriVerilecekTutarıHesapla { get; }

        public ICommand İndirimUygula { get; }

        public Sipariş SeçiliSipariş { get; set; }

        public Siparişler Siparişler { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ÖdemeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SeçiliSipariş" && SeçiliSipariş is not null)
            {
                SeçiliSipariş.PropertyChanged += SeçiliSipariş_PropertyChanged;
            }
        }

        private void SeçiliSipariş_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "ÖdemeŞekli")
            {
            }
            if ((e.PropertyName is "İndirimSeçili") && SeçiliSipariş.İndirimSeçili)
            {
                SeçiliSipariş.İndirimliFiyat = SeçiliSipariş.NormalFiyat = SeçiliSipariş.FiyatHesapla();
            }
            if (e.PropertyName is "İndirimliFiyat" or "İndirimSeçili" or "Adet")
            {
                foreach (Sipariş sipariş in Siparişler.Sipariş)
                {
                    sipariş.NormalFiyat = sipariş.FiyatHesapla();
                    if (sipariş.İndirimSeçili)
                    {
                        sipariş.İndirimUygulandı = true;
                        sipariş.NormalFiyat = sipariş.İndirimliFiyat;
                    }
                }
                Siparişler.ToplamTutar = Siparişler.Sipariş.Sum(z => z.NormalFiyat);
            }
        }
    }
}