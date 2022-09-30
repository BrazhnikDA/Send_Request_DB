using System;
using System.IO;
using System.Windows;

namespace Send_request.Model
{
    class SettingsDBConnection
    {
        string server;
        string port;
        string login;
        string password;
        string nameDB;

        public SettingsDBConnection()
        {
            server = "";
            port = "3306";
            login = "";
            password = "";
            nameDB = "";
        }

        public SettingsDBConnection(string _server, string _port, string _login, string _password, string _nameDB)
        {
            server = _server;
            port = _port;
            login = _login;
            password = _password;
            nameDB = _nameDB;
        }

        public string Server { get => server; set => server = value; }
        public string Port { get => port; set => port = value; }
        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
        public string NameDB { get => nameDB; set => nameDB = value; }

        public string Get_Settings()
        {
            return server + ";" + port + ";" + login + ";" + nameDB + ";" + password;
        }



        public SettingsDBConnection Get_Settings(string path)
        {
            try
            {
                Crypt crypt = new Crypt();

                var sr = new StreamReader(path);
                string buffer = sr.ReadToEnd();
                sr.Close();
                string[] data = buffer.Split(';');

                if ((data[0] == "") || (data[1] == "") || (data[2] == "") || (data[3] == "") || (data[4] == "") ||
                   (data[0] == " ") || (data[1] == " ") || (data[2] == " ") || (data[3] == " ") || (data[4] == " "))
                {
                    return new SettingsDBConnection();
                }

                SettingsDBConnection save = new SettingsDBConnection(data[0], data[1], data[2], crypt.Encrypt_Password(data[3]), data[4]);
                return save;
            }
            catch
            {
                SettingsDBConnection save = new SettingsDBConnection();
                return save;
            }
        }

        public void Set_Settings(string _server, string _port, string _login, string _password, string _nameDB)
        {
           if (_server == "" || _port == "" || _login == "" || _password == "" || _nameDB == "" ||
               _server == " " || _port == " " || _login == " " || _password == " " || _nameDB == " ")
            {
                MessageBox.Show("Не все поля были заполненны!", "Ошибка");
                return;
            }

            string path = Directory.GetCurrentDirectory();
            if (!Directory.Exists(path + "/Settings")) //если папки нет - создаем
            {
                Directory.CreateDirectory(path + "/Settings");
            }
            FileStream fs = File.Create(path + "/Settings/config.txt");
            StreamWriter writer = new StreamWriter(fs);

            Crypt crypt = new Crypt();

            writer.WriteLine(_server + ";" + _port + ";" + _login + ";" + crypt.Crypt_Paswword(_password) + ";" + _nameDB);
            writer.Close();
            MessageBox.Show("Настройки были успешно сохранены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
