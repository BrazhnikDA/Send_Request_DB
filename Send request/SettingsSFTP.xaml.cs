using System;
using System.IO;
using System.Windows;
using Send_request.Model;

namespace Send_request
{
    /// <summary>
    /// Логика взаимодействия для SettingsSFTP.xaml
    /// </summary>
    public partial class SettingsSFTP : Window
    {
        private SettingsSFTPConnection settings;
        public SettingsSFTP()
        {
            settings = new SettingsSFTPConnection();
            InitializeComponent();
            try
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Settings"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Settings");
                }

                var sr = new StreamReader(Directory.GetCurrentDirectory() + "/Settings/sftp.txt");

                String buffer = sr.ReadToEnd();
                sr.Close();
                String[] data = buffer.Split(';');

                Crypt crypt = new Crypt();

                TextBoxHost.Text = crypt.Encrypt_Password(data[0]);
                TextBoxLogin.Text = crypt.Encrypt_Password(data[1]);
                TextBoxPassword.Password = crypt.Encrypt_Password(data[2]);
                TextBoxPort.Text = data[3];
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                settings.Set_Settings(TextBoxHost.Text, TextBoxLogin.Text, TextBoxPassword.Password, TextBoxPort.Text);
                Close();
            }
            catch
            {
                MessageBox.Show("Настройки не сохранены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
