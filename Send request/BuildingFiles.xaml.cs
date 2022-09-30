using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Send_request
{ 
    public partial class BuildingFiles : Window
    {
        List<string> allID = new List<string>() { "" };
        List<string> allSectors = new List<string>() { "" };
        List<string> allAmount = new List<string>() { "" };

        private string zapros = "";
        private string pathFolder = "";
        private bool IsCheckFiles = false;

        public BuildingFiles()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Get_Path())
            {
                Visibly_btnPath(true);
                BuildFiles();
            }else { }
            
        }

        private void BuildFiles()
        {
            int countCorrectFiles = 0;
            List<string> filesname = Directory.GetFiles(pathFolder, "*.txt").ToList<string>();
           
            List<int> allSum = new List<int>() { };

            if (filesname.Count == 0) { Console.Text += "Текстовых файлов не найдено!!!\n"; }

            int j = 0;
            for (int indexFile = 0; indexFile < filesname.Count; indexFile++)
            {
                var sr = new StreamReader(filesname[indexFile]);
                String buffer = sr.ReadToEnd();
                sr.Close();
                String[] data = buffer.Split(';');
                if ((data.Length < 15) || (data[0] != "mail") || (data[1] != "region"))
                {
                    Console.Text += "\n!!!Был встречен неправильный файл: " + filesname[indexFile] + "\n";
                    continue;
                }
                countCorrectFiles++;       // Если мы дошли до сюда значит файл корректный            

                for (int i = 15; i < data.Length; i += 9)
                {
                    allID.Add(data[i]);
                }
                int tmpSum = 0;
                for (int i = 14; i < data.Length; i += 9)
                {
                    int tmp_ = (int.Parse(data[i]) / 100);
                    tmpSum += tmp_;
                    allAmount.Add(tmp_.ToString());
                }
                for (int i = 16; i < data.Length; i += 9)
                {
                    allSectors.Add(data[i]);
                }

                allID.Add("|");
                allAmount.Add("|");
                allSectors.Add("|");

                allSum.Add(tmpSum);

                String[] nameFile = filesname[indexFile].Split('\\');
                int tmp = nameFile.Length - 1;
                Console.Text += nameFile[tmp] + " - " + allSum[j] + "\n";
                Console.Text += "- - - - - - - - - - - - - - - - - - - -\n";
                j++;
            }
       
            if (countCorrectFiles == 0)
            {
                Console.Text += "Коректных файлов не найдено.";
                IsCheckFiles = true;
                btnContinue.Content = "Выход";
            }
            else
            {
                zapros = "SELECT\n" +
                        "from_unixtime(o.created, '%d.%m.%Y %H:%i:%s') as Создана, \n" +
                        "s.number AS Карта, \n" +
                        "s.sector AS Сектор, \n" +
                        "o.oid AS Операция, \n" +
                        "o.amount/100 AS Сумма\n" +
                        "FROM operations o \n" +
                        "inner join sectors s on s.sid = o.purpose \n" +
                        "WHERE \n" +
                        "o.status = 2 \n" +
                        "AND s.number IN( \n";

                for (int i = 1; i < allID.Count - 1; i++)
                {
                    if (i == allID.Count - 2) { zapros += "'" + allID[i] + "')\n"; continue; }
                    if (allID[i] == "|") { continue; }
                    zapros += "'" + allID[i] + "',\n";
                }
                zapros += "order by o.oid desc ";
            }
        }

        private void Text_change(object sender, TextChangedEventArgs e)
        {
            Console.SelectionStart = Console.Text.Length;
            Console.ScrollToEnd();
        }

        private void Click_Continue(object sender, RoutedEventArgs e)
        {
            if (IsCheckFiles)
            {
                Close();
            }
            MainWindow mainWindow = new MainWindow(zapros, pathFolder, allID, allSectors, allAmount);
            mainWindow.Show();
            Close();
        }

        private void btnContinue_MouseLeave(object sender, MouseEventArgs e)
        {
            btnContinue.Opacity = 1;
            btnContinue.Width = 160;
            btnContinue.Height = 40;
            btnContinue.Margin = new Thickness(615, 360, 0, 0);
        }

        private void btnContinue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnContinue.Opacity = 0.85;
            btnContinue.Width = 155;
            btnContinue.Height = 35;
            btnContinue.Margin = new Thickness(620, 365, 0, 0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialogDirectory = new CommonOpenFileDialog { IsFolderPicker = true })
            {
               pathFolder = dialogDirectory.ShowDialog() == CommonFileDialogResult.Ok ?
                                dialogDirectory.FileName : null;

                Save_Path();
                Visibly_btnPath(true);
                BuildFiles();
            }
        }

        private void Save_Path()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Settings")) //если папки нет - создаем
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Settings");
            }
            FileStream fs = File.Create(Directory.GetCurrentDirectory() + "/Settings/path.txt");
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine(pathFolder);
            writer.Close();
        }

        private Boolean Get_Path()
        {
            try
            {
                var sr = new StreamReader(Directory.GetCurrentDirectory() + "/Settings/path.txt");
                String buffer = sr.ReadToEnd();
                sr.Close();

                if (buffer.Length < 3)
                {
                    return false;
                }
                pathFolder = buffer.Remove(buffer.Length - 2, 2);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Visibly_btnPath(bool IsActive) // true - скрыть / false - показать
        {
            if(IsActive)
            {
                btnPath.Visibility = Visibility.Hidden;
                BorderForbtnPath.Visibility = Visibility.Hidden;
            }else
            {
                btnPath.Visibility = Visibility.Visible;
                BorderForbtnPath.Visibility = Visibility.Visible;
            }
        }
    }
}
