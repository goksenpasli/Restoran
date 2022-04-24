using System.ComponentModel;
using System.Windows.Documents;

namespace Restoran.ViewModel
{
    public class DocumentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Başlık { get; set; }

        public FixedDocumentSequence Document { get; set; }
    }
}