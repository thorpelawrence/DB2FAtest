using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Google.Authenticator;

namespace DB2FAtest
{
    /// <summary>
    /// Interaction logic for _2FactorSetupWindow.xaml
    /// </summary>
    public partial class TwoFactorSetupWindow : Window
    {
        TwoFactorAuthenticator tfa;
        string key;

        public TwoFactorSetupWindow(string username, string key)
        {
            InitializeComponent();
            this.key = key;
            tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("Redis test wpf2", "Redis test wpf2 " + username, key, 300, 300);
            image.Source = new BitmapImage(new Uri(setupInfo.QrCodeSetupImageUrl));
            label.Content = "Manual entry key: " + setupInfo.ManualEntryKey;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (tfa.ValidateTwoFactorPIN(key, textBox.Text.Replace(" ", "")))
            {
                DialogResult = true;
                Close();
            }
            else textBox.Background = Brushes.Red;
        }
    }
}
