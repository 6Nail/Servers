using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var client = new TcpClient())
            {
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3231));
                using (var stream = client.GetStream())
                {
                    var data = Encoding.UTF8.GetBytes("GET_FILES*");
                    stream.Write(data, 0, data.Length);
                }
            }

            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 801);
            listener.Start();

            using (var client = listener.AcceptTcpClient())
            {
                using (var stream = client.GetStream())
                {
                    var resultText = string.Empty;
                    while (stream.DataAvailable)
                    {
                        var buffer = new byte[65535];
                        stream.Read(buffer, 0, buffer.Length);
                        resultText = Encoding.UTF8.GetString(buffer);
                        string[] fileNames = resultText.Split(new char[] { '*' });
                        for (int i = 0; i < fileNames.Count() - 1; i++)
                        {
                            fileList.Items.Add(fileNames[i]);
                        }
                    }

                }
            }

            if (fileList.Items.Count == 0)
            {
                fileList.Items.Add("Вы не добавили ни одного файла");
                fileList.IsEnabled = false;
            }
            deleteBtn.IsEnabled = false;
        }

        private void IsNullItem(object sender, MouseButtonEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                deleteBtn.IsEnabled = true;
            }
        }

        private void AddFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                fileList.IsEnabled = true;
                if (fileList.Items[0].ToString() == "Вы не добавили ни одного файла")
                {
                    fileList.Items.Clear();
                }
                string[] temp = openFileDialog.FileName.Split(new char[] { '\\' });
                bool isBeFile = false;
                foreach (var item in fileList.Items)
                {
                    if (item.ToString() == temp[temp.Length - 1])
                    {
                        isBeFile = true;
                        MessageBox.Show("Файл уже добавлен!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                if (!isBeFile)
                {
                    fileList.Items.Add(temp[temp.Length - 1]);
                    using (var client = new TcpClient())
                    {
                        client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3231));
                        using (var stream = client.GetStream())
                        {
                            var data = Encoding.UTF8.GetBytes("ADD_FILE*" + openFileDialog.FileName + "*");
                            stream.Write(data, 0, data.Length);
                        }
                    }
                }
            }
        }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                using (var client = new TcpClient())
                {
                    client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3231));
                    using (var stream = client.GetStream())
                    {
                        var data = Encoding.UTF8.GetBytes("DELETE_FILE*" + fileList.SelectedItem + "*");
                        stream.Write(data, 0, data.Length);
                    }
                }

                fileList.Items.Remove(fileList.SelectedItem);
                if (fileList.Items.Count == 0)
                {
                    fileList.Items.Add("Вы не добавили ни одного файла");
                    deleteBtn.IsEnabled = false;
                    fileList.IsEnabled = false;
                }
            }

        }
    }
}
