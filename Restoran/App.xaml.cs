using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using HandyControl.Controls;

namespace Restoran
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDown));
            EventManager.RegisterClassHandler(typeof(ComboBox), UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDown));
            EventManager.RegisterClassHandler(typeof(NumericUpDown), UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDown));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            FrameworkContentElement.LanguageProperty.OverrideMetadata(typeof(System.Windows.Documents.TextElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private static void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((sender as TextBox)?.AcceptsReturn == false || sender is NumericUpDown || sender is ComboBox)
                {
                    MoveToNextUiElement(e);
                }
            }
        }

        private static void MoveToNextUiElement(RoutedEventArgs e)
        {
            TraversalRequest request = new(FocusNavigationDirection.Next);
            if (!(Keyboard.FocusedElement is not UIElement elementWithFocus) && elementWithFocus.MoveFocus(request))
            {
                e.Handled = true;
            }
        }
    }
}