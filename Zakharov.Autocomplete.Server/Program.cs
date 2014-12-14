using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zakharov.Autocomplete.Server {
    class Program {
        static void Main(string[] args) {
            #region Проверим переданные параметры командной строки
            if (args.Length != 2)
                throw new ArgumentException("При запуске приложения необходимо передать в параметрах командной строки имя файла, содержащего частотный словарь, и номер порта", "args");
            int port = 0;
            if (!Int32.TryParse(args[1], out port))
                throw new ArgumentException("Номер порта должен быть целым числом","args[1]");
            #endregion            
            // Создаем и запускаем TCP-сервер
            Server TcpServer = new Server();
            TcpServer.Start(args[0],port);
            Console.WriteLine("Press enter to continue...");
            Console.Read();
            // Останавливаем TCP-сервер
            TcpServer.Stop();            
        }
    }
}
