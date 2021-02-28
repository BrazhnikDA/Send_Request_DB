using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Send_request.Model
{
    class SettingsSFTPConnection
    {
        string host;
        string username;
        string password;
        string port;

        public SettingsSFTPConnection()
        {
            host = "";
            username = "";
            password = "";
            port = "";
        }

        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        public SettingsSFTPConnection(string _host, string _username, string _password, string _port)
        {
            this.host = _host;
            this.username = _username;
            this.password = _password;
            this.port = _port;
        }

        public SettingsSFTPConnection Get_Settings(string path)
        {
            try
            {
                var sr = new StreamReader(path);
                string buffer = sr.ReadToEnd();
                sr.Close();
                string[] data = buffer.Split(';');

                if ((data[0] == "") || (data[1] == "") || (data[2] == "") || (data[3] == "") ||
                   (data[0] == " ") || (data[1] == " ") || (data[2] == " ") || (data[3] == " "))
                {
                    
                }

                Crypt crypt = new Crypt();

                SettingsSFTPConnection save = new SettingsSFTPConnection(crypt.Encrypt_Password(data[0]), crypt.Encrypt_Password(data[1]),
                    crypt.Encrypt_Password(data[2]), data[3]);
                return save;
            }
            catch
            {
                SettingsSFTPConnection save = new SettingsSFTPConnection();
                return save;
            }
        }

        public void Set_Settings(string _host, string _username, string _password, string _port)
        {
            
            if ((_host == "") || (_username == "") || (_password == "") || (_port == "") ||
                (_host == " ") || (_username == " ") || (_password == " ") || (_port == " "))
            {
                MessageBox.Show("Не все поля были заполненны!", "Ошибка");
                return;
            }
            

            Crypt crypt = new Crypt();

            string path = Directory.GetCurrentDirectory();
            if (!Directory.Exists(path + "/Settings")) //если папки нет - создаем
            {
                Directory.CreateDirectory(path + "/Settings");
            }
            FileStream fs = File.Create(path + "/Settings/sftp.txt");
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine(crypt.Crypt_Paswword(_host) + ";" + crypt.Crypt_Paswword(_username) + ";" + crypt.Crypt_Paswword(_password) + ";" + _port);
            writer.Close();
            MessageBox.Show("Настройки были успешно сохранены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

