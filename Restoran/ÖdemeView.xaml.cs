using System.Windows.Controls;
using HandyControl.Data;
using Restoran.Model;

namespace Restoran
{
    /// <summary>
    /// Interaction logic for ÖdemeView.xaml
    /// </summary>
    public partial class ÖdemeView : UserControl
    {
        public ÖdemeView()
        {
            InitializeComponent();
        }

        private void NumericAdet_ValueChanged(object sender, FunctionEventArgs<double> e)
        {
            Siparişler dc = DataContext as Siparişler;
            dc.ToplamTutar = dc.Sipariş.SiparişToplamları();
        }
    }
}