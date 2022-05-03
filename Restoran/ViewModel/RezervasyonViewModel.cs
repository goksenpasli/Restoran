using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class RezervasyonViewModel : INotifyPropertyChanged
    {
        public RezervasyonViewModel()
        {
            MüşteriKaydet = new RelayCommand<object>(parameter =>
            {
                if (parameter is MainViewModel mainViewModel)
                {
                    Rezervasyonlar rezervasyon = new()
                    {
                        Telefon = Telefon,
                        Ad = Ad,
                        Soyad = Soyad,
                        Id = ExtensionMethods.RandomNumber(),
                        RezervasyonTarihi = RezervasyonTarihi,
                        MasaId = SeçiliMasa.Id,
                    };
                    SeçiliMasa.Rezervasyonlar.Add(rezervasyon);
                    mainViewModel.RezervasyonListeleri.Add(rezervasyon);
                    SeçiliMasa.Rezerve = true;
                    mainViewModel.DatabaseSave.Execute(null);
                    Reset();
                }
            }, parameter => !string.IsNullOrWhiteSpace(Ad) && !string.IsNullOrWhiteSpace(Soyad) && !string.IsNullOrWhiteSpace(Telefon));

            MüşteriSil = new RelayCommand<object>(parameter =>
            {
                if (parameter is MainViewModel mainViewModel)
                {
                    mainViewModel.RezervasyonListeleri.Remove(Rezervasyon);
                    SeçiliMasa.Rezervasyonlar.Remove(Rezervasyon);
                    if (SeçiliMasa.Rezervasyonlar.All(z=>z.RezervasyonTarihi <= DateTime.Now))
                    {
                        SeçiliMasa.Rezerve = false;
                    }
                    mainViewModel.DatabaseSave.Execute(null);
                    Reset();
                }
            }, parameter => Rezervasyon is not null);

            RezervasyonKaydet = new RelayCommand<object>(parameter =>
            {
                if (parameter is MainViewModel mainViewModel)
                {
                    mainViewModel.MasaSiparişArtır.Execute(SeçiliÜrün);
                }
            }, parameter => true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Ad { get; set; }

        public ICommand MüşteriKaydet { get; set; }

        public ICommand MüşteriSil { get; set; }

        public Rezervasyonlar Rezervasyon { get; set; }

        public ICommand RezervasyonKaydet { get; set; }

        public DateTime RezervasyonTarihi { get; set; } = DateTime.Today;

        public Masa SeçiliMasa { get; set; }

        public Ürün SeçiliÜrün { get; set; }

        public string Soyad { get; set; }

        public string Telefon { get; set; }

        private void Reset()
        {
            Telefon = null;
            Ad = null;
            Soyad = null;
        }
    }
}