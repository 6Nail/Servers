using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists(@"Files"))
            {
                Directory.CreateDirectory(@"Files");
            }
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3231);
            listener.Start();
            Console.WriteLine($"Сервер для файлового менеджера запущен {DateTime.Now}");
            using (var context = new Context())
            {
                context.SaveChanges();
            }
            while (true)
            {
                using (var client = listener.AcceptTcpClient())
                {
                    using (var stream = client.GetStream())
                    {
                        var resultText = string.Empty;
                        while (stream.DataAvailable)
                        {
                            var buffer = new byte[24];
                            stream.Read(buffer, 0, buffer.Length);
                            resultText += Encoding.UTF8.GetString(buffer);
                        }
                        //Console.WriteLine($"Данные от клиента - {resultText}");
                        string[] typeTask = resultText.Split(new char[] { '*' });
                        if (typeTask[0] == "GET_FILES")
                        {
                            Console.WriteLine($"[{DateTime.Now}]\tЗапрос на получение всех файлов");
                            string[] dirs = Directory.GetFiles(@"Files");
                            using (var clientSend = new TcpClient())
                            {

                                clientSend.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 801));
                                using (var streamSend = clientSend.GetStream())
                                {
                                    foreach (var item in dirs)
                                    {
                                        string[] fileName = item.Split(new char[] { '\\' });
                                        var data = Encoding.UTF8.GetBytes(fileName[1] + "*");
                                        streamSend.Write(data, 0, data.Length);
                                    }
                                }
                            }
                            Console.WriteLine($"[{DateTime.Now}]\tУспешно!\n\n");
                        }
                        else if (typeTask[0] == "ADD_FILE")
                        {
                            Console.WriteLine($"[{DateTime.Now}]\tЗапрос на добавление файла");
                            string[] fileName = typeTask[1].Split(new char[] { '\\' });
                            File.Copy(typeTask[1], @"Files\" + fileName[fileName.Length - 1], false);
                            using (var context = new Context())
                            {
                                var file = new FileDb()
                                {
                                    Name = fileName[fileName.Length - 1]
                                };
                                context.Files.Add(file);
                                context.SaveChanges();
                            }

                            Console.WriteLine($"[{DateTime.Now}]\tУспешно!\n\n");
                        }
                        else if (typeTask[0] == "DELETE_FILE")
                        {
                            Console.WriteLine($"[{DateTime.Now}]\tЗапрос на удаление файла");
                            File.Delete(@"Files\" + typeTask[1]);
                            using (var context = new Context())
                            {
                                var file = context.Files.Where(f => f.Name == typeTask[1].ToString()).FirstOrDefault();
                                context.Files.Remove(file);
                                context.SaveChanges();
                            }
                            Console.WriteLine($"[{DateTime.Now}]\tУспешно!\n\n");
                        }
                    }
                    Console.WriteLine("Соединение закрыто");
                }


            }
        }
    }
}
