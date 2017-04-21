using Sodium;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Sodium.PasswordHash;
using Google.Authenticator;

namespace DB2FAtest
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        ConnectionMultiplexer redis;
        IDatabase db;

        public string username;

        public LoginWindow()
        {
            InitializeComponent();
            redis = ConnectionMultiplexer.Connect("redis-17057.c8.us-east-1-4.ec2.cloud.redislabs.com:17057");
            db = redis.GetDatabase();
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            username = usernameTextBox.Text;
            if (!db.HashExists("users", username))
            {
                MessageBox.Show("Username does not exist");
                return;
            }

            long userID = (long)db.HashGet("users", username);
            if (ArgonHashStringVerify(db.HashGet("user:" + userID, "password"), passwordBox.Password))
            {
                bool passwordCorrect = false;
                if (db.HashExists("user:" + userID, "twofactorauthkey"))
                {
                    if (new TwoFactorVerifyWindow(db.HashGet("user:" + userID, "twofactorauthkey")).ShowDialog() ?? false)
                    {
                        passwordCorrect = true;
                    }
                }
                else
                {
                    passwordCorrect = true;
                }
                if (passwordCorrect)
                {
                    DialogResult = true;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Incorrect password");
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().Show();
        }

    }
}
