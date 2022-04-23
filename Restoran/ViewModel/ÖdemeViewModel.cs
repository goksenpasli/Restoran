using System.ComponentModel;
using Restoran.Model;

namespace Restoran.ViewModel
{
    public class ÖdemeViewModel : INotifyPropertyChanged
    {
        public ÖdemeViewModel()
        {
            PropertyChanged += ÖdemeViewModel_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Sipariş SeçiliSipariş { get; set; }

        public Siparişler Siparişler { get; set; }

        private void ÖdemeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SeçiliSipariş" && SeçiliSipariş is not null)
            {
                SeçiliSipariş.PropertyChanged += SeçiliSipariş_PropertyChanged;
            }
        }

        private void SeçiliSipariş_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "Adet")
            {
                Siparişler.ToplamTutar = Siparişler.Sipariş.SiparişToplamları();
            }
            if (e.PropertyName is "ÖdemeŞekli")
            {
            }
        }
    }
}