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
using Sodium;
using StackExchange.Redis;
using static Sodium.PasswordHash;

namespace DB2FAtest
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var redis = ConnectionMultiplexer.Connect("redis-17057.c8.us-east-1-4.ec2.cloud.redislabs.com:17057");
            var db = redis.GetDatabase();
            if (string.IsNullOrEmpty(usernameTextBox.Text) || string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Must enter username and password");
                return;
            }
            if (passwordBox.Password != passwordBox2.Password)
            {
                MessageBox.Show("Passwords must match");
                return;
            }
            string username = usernameTextBox.Text;
            string password = ArgonHashString(passwordBox.Password);

            if (db.HashExists("users", username))
            {
                MessageBox.Show("Username already exists");
                return;
            }

            long userID = db.StringIncrement("next_user_id");
            List<HashEntry> userEntry = new List<HashEntry>();
            userEntry.Add(new HashEntry("username", username));
            userEntry.Add(new HashEntry("password", password));
            if (checkBox.IsChecked ?? false)
            {
                var key = Encoding.UTF8.GetString(ArgonGenerateSalt());
                if (new TwoFactorSetupWindow(username, key).ShowDialog() ?? false)
                    userEntry.Add(new HashEntry("twofactorauthkey", key));
                else return;
            }
            db.HashSet("users", username, userID);
            db.HashSet("user:" + userID, userEntry.ToArray());
            Close();
        }
    }
}
