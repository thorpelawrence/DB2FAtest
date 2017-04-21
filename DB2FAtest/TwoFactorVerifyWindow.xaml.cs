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
    /// Interaction logic for TwoFactorVerifyWindow.xaml
    /// </summary>
    public partial class TwoFactorVerifyWindow : Window
    {
        string key;
        public TwoFactorVerifyWindow(string key)
        {
            InitializeComponent();
            this.key = key;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            if (tfa.ValidateTwoFactorPIN(key, textBox.Text))
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
