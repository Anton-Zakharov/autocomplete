using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zakharov.Autocomplete.Server {
    /// <summary>
    /// Сетевой сервер
    /// </summary>
    public class Server {
        /// <summary>TCP-сервер</summary>
        private TcpListener TcpServer = null;
        /// <summary>Основной поток, в котором работает TCP-сервер</summary>
        private Thread Main = null;
        private Zakharov.Autocomplete.Model.Dictionary Words = null;

        /// <summary>
        /// Создание объекта
        /// </summary>
        public Server() {

        }
        /// <summary>
        /// Запуск сервера
        /// </summary>
        /// <param name="fileName">имя текстового файла, содержащего частотный словарь</param>
        /// <param name="port">номер порта</param>
        public void Start(string fileName, int port) {           
            // Создаем частотный словарь из текстового файла
            Words = new Zakharov.Autocomplete.Model.Dictionary(10);
            try {
                StreamReader stream = new StreamReader(new FileStream(fileName, FileMode.Open));
                // Получаем количество слов в частотном словаре
                int wordsCount = Int32.Parse(stream.ReadLine());
                #region Заполнение частотного словаря
                for (int i = 0; i < wordsCount; i++) {
                    string line = stream.ReadLine();
                    if (line == null)
                        throw new Exception(String.Format("Текстовой файл {0} должен содержать {1} строк",wordsCount,fileName));
                    string[] word = line.Split(' ');
                    if (word.Length != 2)
                        throw new Exception("Входная строка частотного словаря должна иметь вид: слово частота");
                    if (word[0].Length > 15)
                        throw new Exception("Слово должно содержать не более 15 символов");
                    int frequency = 0;
                    if (!Int32.TryParse(word[1], out frequency) || frequency <= 0)
                        throw new Exception("Частота должна быть целым положительным числом");
                    Words.AddWord(word[0], frequency);
                }
                #endregion
            }
            catch (IOException error) {
                Console.WriteLine("IO error: {0}", error.Message);
            }           
            // Создаем новый поток для прослушивания соединений
            Main = new Thread(this.Listening);
            Main.Start(port);
        }
        /// <summary>
        /// Прослушивание соединений
        /// </summary>
        /// <param name="port">номер порта</param>
        private void Listening(object port) {
            try {
                // Создаем и запускаем TCP-сервер для прослушивания соединений по заданному порту на всех доступных сетевых интерфейсах.
                Console.WriteLine("Starting server...");               
                TcpServer = new TcpListener(IPAddress.Any, (int)port);
                TcpServer.Start();
                Console.WriteLine("Server started");
                #region Ожидание и обработка входящих соединений
                while (true) {
                    // Ожидаем входящего соединения
                    if (TcpServer.Pending()) {
                        // Запускаем отдельный поток для обработки входящего соединения
                        (new Thread(this.Processing)).Start(TcpServer.AcceptTcpClient());                        
                    }
                    Thread.Sleep(1000);
                }
                #endregion
            }
            catch (SocketException SocketError) {
                Console.WriteLine("Socket error: {0}", SocketError.Message);
            }
            catch (ThreadAbortException AbortError) {
                
            }
            finally {
                // Останавливаем TCP-сервер
                TcpServer.Stop();
                Console.WriteLine("Server stoped");
            }
        }
        /// <summary>
        /// Остановка сервера
        /// </summary>
        public void Stop() {
            if (Main != null && Main.ThreadState != ThreadState.Unstarted) {
                Console.WriteLine("Stoping server...");
                // Останавливаем поток для прослушивания соединений.
                Main.Abort();
                // Дожидаемся завершения потока.
                Main.Join();
            }
        }
        private void Processing(object client) {
            Byte[] buffer = new Byte[256];
            String request = "";

            try {
                Console.WriteLine("Client connected");
                // Получаем поток с данными
                NetworkStream stream = (client as TcpClient).GetStream();
                Console.WriteLine("Processing request...");
                #region Прочитываем все переданные данные
                while (stream.DataAvailable) {
                    // Считываем данные в буфер байтов
                    int length = stream.Read(buffer, 0, buffer.Length);
                    // Преобразуем массив байт в строку ASCII
                    request += System.Text.Encoding.ASCII.GetString(buffer, 0, length);                   
                }
                #endregion
                // Выделяем в запросе префикс слова
                if (request.Length > 4 && request.Substring(0, 4).ToLower() == "get ") {
                    string prefix = request.Substring(4);
                    // Получаем из частотного словаря список подходящих под префикс слов
                    List<string> autocompletes= Words.GetAutocomplete(prefix);
                    // Собираем все слова в одну строку
                    StringBuilder messages = new StringBuilder();
                    foreach (string word in autocompletes)
                        messages.Append(word+"\n");
                    // Отправляем список подходящих слов клиенту
                    byte[] message = System.Text.Encoding.ASCII.GetBytes(messages.ToString());
                    stream.Write(message, 0, message.Length);
                }
                // Закрываем соединение с клиентом
                (client as TcpClient).Close();
                Console.WriteLine("Client closed");
            }
            catch (Exception Error) {
                Console.WriteLine("Processing error: {0}", Error.Message);
            }
        }
    }
}
