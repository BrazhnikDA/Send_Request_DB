using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using Send_request.Model;

namespace Send_request
{
    public partial class MainWindow : Window
    {
        private SettingsDBConnection   settings;                    // Хранит поля подключения
        private BindingList<ListModel> _todoData;                   // Таблица
       
        private string  connectionInfo;                             // Строка для подключения к БД
        private bool    IsConnection;                               // true - подключенно / false - неудача
        private string  zapros;                                     // Строка для хранения запроса
        private string  pathForCreateFile;                          // Путь до созданного файла
        private string  pathForFolder;

        List<string> allID      = new List<string>() { "" };         // Для хранения всех numbers из файла 
        List<string> allSectors = new List<string>() { "" };         // Для хранения всех sectors из файла
        List<string> allAmount  = new List<string>() { "" };         // Для хранения всех amount  из файла

        List<string> zaprosID       = new List<string>() { "" };     // Для хранения numbers после запроса в БД
        List<string> zaprosSectors  = new List<string>() { "" };     // Для хранения sectors после запроса в БД
        List<string> zaprosAmount   = new List<string>() { "" };     // Для хранения amount  после запроса в БД

        public MainWindow(string zapros_, string pathSourceFiles, List<string> id, List<string> sectors, List<string> amount)
        {
            InitializeComponent();

            SaveTextFile(zapros_, "Запрос", pathSourceFiles);


            pathForFolder = pathSourceFiles;        // Путь до исходных файлов содержит начало пути до нужной папки

            allID = id;
            allSectors = sectors;
            allAmount = amount;

            settings = new SettingsDBConnection();
            
            Get_Date();
            zapros = zapros_;
        }

        public MainWindow(string zapros_)
        {
            InitializeComponent();
            settings = new SettingsDBConnection();

            Get_Date();
            zapros = zapros_;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _todoData = new BindingList<ListModel>() { };
            dgPassList.ItemsSource = _todoData;

            SetSettings();
            Connect_DataBase();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Connect_DataBase();
            btnCheck.Background = new SolidColorBrush(Colors.White);
        }

        private void SetSettings()
        {
            try
            {
                settings = settings.Get_Settings(Directory.GetCurrentDirectory() + "/Settings/config.txt");

                connectionInfo = "server=" + settings.Server + ";" + "port=" + settings.Port + ";" + "user=" + settings.Login + ";" +
                    "DataBase=" + settings.NameDB + ";" + "password=" + settings.Password + ";";
            }
            catch
            {
                VisiblyButton(false);
            }
        }

        private void VisiblyButton(bool IsCheck)
        {
            if (IsCheck == false)
            {
                buttonStart.Visibility = Visibility.Visible;
                buttonStart.Width = 100;
                buttonStart.Height = 35;
                buttonStart.Margin = new Thickness(0, 10, 20, 10);
                buttonStart.FontSize = 14;
                buttonStart.Content = "Запрос";
                buttonStart.Background = Brushes.LightGray;
            }
            else
            {
                buttonStart.Visibility = Visibility.Hidden;
            }
        }

        public void Connect_DataBase()
        {
            // Перед новым запросом чистим память
            _todoData.Clear();
            zaprosID.Clear();
            zaprosSectors.Clear();
            zaprosAmount.Clear();
            try
            {
                buttonStart.Visibility = Visibility.Hidden;
                SetSettings();
                DataTable dataTable = new DataTable("Sectors_vyborka");

                MySqlConnection connect = new MySqlConnection(connectionInfo);

                connect.Open();

                MySqlCommand sqlCommand = connect.CreateCommand();

                sqlCommand.CommandText = zapros;

                MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataTable);

                string tmpZapros = "";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    tmpZapros += dataTable.Rows[i][1].ToString() + " " + dataTable.Rows[i][2].ToString() + " " + dataTable.Rows[i][4].ToString() + "\n";
                    _todoData.Add(new ListModel()
                    {
                        Date = dataTable.Rows[i][0].ToString(),
                        Card = dataTable.Rows[i][1].ToString(),
                        Sector = dataTable.Rows[i][2].ToString(),
                        Operations = dataTable.Rows[i][3].ToString(),
                        Summa = dataTable.Rows[i][4].ToString()
                    });
                    zaprosID.Add(dataTable.Rows[i][1].ToString());
                    zaprosSectors.Add(dataTable.Rows[i][2].ToString());
                    zaprosAmount.Add(dataTable.Rows[i][4].ToString());
                }

                SaveTextFile(tmpZapros, "Таблица после запроса", pathForFolder); // Сохранить запрос

                dgPassList.ItemsSource = _todoData;

                if (!Directory.Exists(pathForFolder + "/Resault")) // Если папки нет - создаем
                {
                    Directory.CreateDirectory(pathForFolder + "/Resault");
                }
                pathForCreateFile = pathForFolder + "/Resault/" + "Z-BAS_OTMENA_YL_" + Get_Date() + "_" + Get_Time() + ".csv";


                FileStream fs = File.Create(pathForCreateFile);
                StreamWriter writer = new StreamWriter(fs);
                writer.WriteLine("number;sector;operation");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 1; j < dataTable.Columns.Count - 1; j++)   // Отрезаем дату и сумму для занесения в .csv
                    {
                        if (j / 3 == 0) { writer.Write(dataTable.Rows[i][j] + ";"); }
                        else
                        {
                            writer.Write(dataTable.Rows[i][j]);
                        }   
                    }
                    writer.Write("\n");
                }
               
                IsConnection = true;
                VisiblyButton(IsConnection);

                // dgPassList.RowBackground = new SolidColorBrush(Colors.White);
                // MessageBox.Show("Строки: " + allID.Count.ToString() + " " + zaprosID.Count.ToString(),"Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                writer.Close();
                connect.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка в подключении к БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                IsConnection = false;
                VisiblyButton(IsConnection);
                Settings settings = new Settings();
                settings.Show();
                return;
            }
        }

        public void Set_Zapros(string zapros_)
        {
            zapros = zapros_;
        }

        public string Get_Date()
        {
            return DateTime.Now.ToString("yyMMdd");     // Возвращает текущюю дату
        }

        public string Get_Time()
        {
            return DateTime.Now.ToString("HHmmss");     // Возвращает текущее время
        }

        private void MenuItem_Click_SFTP(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsSFTPConnection settingsSFTP = new SettingsSFTPConnection();
                settingsSFTP = settingsSFTP.Get_Settings(Directory.GetCurrentDirectory() + "/Settings/sftp.txt");

                string source = pathForCreateFile;
                string destination = @"/var/www/app/file";

                string host = settingsSFTP.Host;
                string username = settingsSFTP.Username;
                string password = settingsSFTP.Password;
                int port = Convert.ToInt32(settingsSFTP.Port);

                try
                {
                    bool IsSend = UploadSFTPFile(host, username, password, source, destination, port);
                    if (!File.Exists(pathForCreateFile))
                    {
                        MessageBox.Show("Файл уже удалён", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if (IsSend)
                        {
                            File.Delete(pathForCreateFile);
                            MessageBox.Show("Файл отправлен и удалён.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            File.Delete(pathForCreateFile);
                            MessageBox.Show("Файл удалён.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch { MessageBox.Show("Файл отмены не удалён!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); } // Удалить файл с результатом
                VisiblyButton(false);
            }
            catch
            {
                MessageBox.Show("Настройки SFTP не найдены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                SettingsSFTP settingsSFTP = new SettingsSFTP();
                settingsSFTP.Show();
            }
        }

        public bool UploadSFTPFile(string host, string username, string password, string sourcefile, string destinationpath, int port)
        {
            try
            {
                using (SftpClient client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    client.ChangeDirectory(destinationpath);
                    using (FileStream fs = new FileStream(sourcefile, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(fs, Path.GetFileName(sourcefile));
                    }
                }
                return true;
            }
            catch
            {
                MessageBox.Show("Соеденение не установленно, файл не был отправлен!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void MenuItem_Click_Settings_BD(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
            VisiblyButton(false);
        }

        private void MenuItem_Click_Settings_SFTP(object sender, RoutedEventArgs e)
        {
            SettingsSFTP settingsSFTP = new SettingsSFTP();
            settingsSFTP.Show();
            VisiblyButton(false);
        }
        
        private void MenuItem_Click_CheckFiles(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В файле: " + ((allID.Count - 1) - FindSeparatorinText()).ToString() + "\nВ запросе: " + (zaprosID.Count - 1).ToString(), "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            // 1 строка у запрос ID = пустота. Поэтому -1

            _todoData.Clear();
            int error = 0;

            string tmpAllIDError   = "";
            string tmpAllIDSuccess = "";

            for (int i = 0; i < allID.Count; i++)
            {
                switch(FindSubStringID(allID[i], allSectors[i], allAmount[i]))
                {
                    case 0:
                        tmpAllIDSuccess += allID[i].ToString() + " " + allSectors[i].ToString() + " " + allAmount[i].ToString() + "\n";
                        break;
                    case 1:
                        break;
                    case -1:
                        _todoData.Add(new ListModel()
                        {
                            Card = allID[i].ToString(),
                            Sector = allSectors[i].ToString(),
                            Summa = allAmount[i].ToString()
                        });
                        tmpAllIDError += allID[i].ToString() + " " + allSectors[i].ToString() + " " + allAmount[i].ToString() + "\n";
                        error++;
                        break;
                    case -100:
                        i = allID.Count;
                        break;
                    default:
                        i = allID.Count;
                        break;
                }
                
            }

            SaveTextFile(tmpAllIDError, "Строки непрошедшие проверку", pathForFolder);
            SaveTextFile(tmpAllIDSuccess, "Хорошие строки", pathForFolder);

            if (error > 0) 
            { 
                btnCheck.Background = new SolidColorBrush(Colors.IndianRed);
                MessageBox.Show("Встречены строки не прошедшие проверку: " + error + " строк", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else 
            { 
                btnCheck.Background = new SolidColorBrush(Colors.LightGreen);
                MessageBox.Show("Все строки прошли проверку!","Успешно", MessageBoxButton.OK, MessageBoxImage.Information);           
            }
            VisiblyButton(false);
        }

        private int FindSubStringID(string ID, string Sector, string Amount)   // 0 - прошла, -1 - ошибка, 1 - палочка или пустота, -100 - вышли за пределы массива
        {
            try
            {
                if (ID == "|" || ID == "")
                    return 1;
                for (int i = 0; i < zaprosID.Count; i++)
                {
                    if (zaprosID[i].ToString() == ID)
                    {
                        if (zaprosSectors[i].ToString() == Sector)
                        {
                            // zaprosAmount[i] = "-2300,500"; - Пример тестовой ситуации
                            // Отделяем целую часть, берём модуль числа, сравниваем с запросом из БД сконвертировав в строку 
                            String[] intPartAmount = zaprosAmount[i].Split(',');
                            int delMinus = Math.Abs(Convert.ToInt32(intPartAmount[0]));
                            if (delMinus.ToString() == Amount)
                            {
                                return 0;
                            }
                        }
                    }
                }
                return -1;
            }catch
            {
                MessageBox.Show("При проверке мы вышли за пределы массива.","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return -100;
            }
        }

        private int FindSeparatorinText()   // Возвращает количество разделителей в тексте
        {
            int count = 0;
            for(int i =  0; i < allID.Count; i++)
            {
                if(allID[i] == "|")
                {
                    count++;
                }
            }
            return count;
        }   

        private bool SaveTextFile(string text, string NameFile, string path)
        {
            try
            {
                if (!Directory.Exists(path + "\\Файлы")) // Если папки нет - создаем
                {
                    Directory.CreateDirectory(path + "\\Файлы");
                }

                FileStream fs = File.Create(path + "\\Файлы" + "\\" + NameFile + ".txt");
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(text);

                writer.Close();
                return true;
            }
            catch
            {
                MessageBox.Show("Файл " + NameFile + " не был сохранён.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
