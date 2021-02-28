using System;
using System.IO;
using System.Windows;
using Send_request.Model;

namespace Send_request
{
    public partial class Settings : Window
    {
        private SettingsDBConnection settings;
        public Settings()
        {
            settings = new SettingsDBConnection();
            InitializeComponent();
            try
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Settings"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Settings");
                }
                
                var sr = new StreamReader(Directory.GetCurrentDirectory() + "/Settings/config.txt");

                String buffer = sr.ReadToEnd();
                sr.Close();
                String[] data = buffer.Split(';');

                Crypt crypt = new Crypt();

                TextBoxServer.Text = data[0];
                TextBoxPort.Text = data[1];
                TextBoxLogin.Text = data[2];
                TextBoxPassword.Password = crypt.Encrypt_Password(data[3]);
                TextBoxDataBase.Text = data[4];
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                settings.Set_Settings(TextBoxServer.Text, TextBoxPort.Text, TextBoxLogin.Text, TextBoxPassword.Password, TextBoxDataBase.Text);
                Close();
            }
            catch
            {
                MessageBox.Show("Настройки не сохранены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
