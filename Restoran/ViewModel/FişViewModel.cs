using System.ComponentModel;
using System.Windows.Documents;

namespace Restoran.ViewModel
{
    public class FişViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FixedDocumentSequence Document { get; set; }
    }
}