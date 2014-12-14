using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zakharov.Autocomplete.Client {
    class Program {
        static void Main(string[] args) {
            #region Проверим переданные параметры командной строки
            if (args.Length != 2)
                throw new ArgumentException("При запуске приложения необходимо передать в параметрах командной строки ip-адрес сервера и номер порта", "args");
            string server = args[0];
            int port = 0;
            if (!Int32.TryParse(args[1], out port))
                throw new ArgumentException("Номер порта должен быть целым числом", "args[1]");
            #endregion                  
            // Запрашиваем у пользователя префикс слов
            System.Console.Write("Input prefix: ");
            string prefix = System.Console.ReadLine();
            try {
                // Создаем TCP-клиента для передачи сообщений серверу по указанному порту
                TcpClient client = new TcpClient(server, port);                
                // Преобразуем строку ASCII в массив байт, добавляя в начало строки команду get
                byte[] data = System.Text.Encoding.ASCII.GetBytes("get "+prefix);
                // Получаем поток с данными
                NetworkStream stream = client.GetStream();
                // Отправляем префикс на сервер 
                stream.Write(data, 0, data.Length);
                // Buffer to store the response bytes.
                byte[] buffer = new byte[256];
                string response = "";
                #region Прочитываем все переданные данные
                int length;
                while ((length = stream.Read(buffer, 0, buffer.Length)) != 0) {
                    // Преобразуем массив байт в строку ASCII
                    response += System.Text.Encoding.ASCII.GetString(buffer, 0, length);
                }
                #endregion
                System.Console.WriteLine("\nAutocomplete:\n"+response);
                // Закрываем поток данных и соединение с сервером
                stream.Close();
                client.Close();
            }
            catch (SocketException SocketError) {
                Console.WriteLine("Socket error: {0}", SocketError.Message);
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.Read();
        }
    }
}
