using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace STS
{
    public enum ResponseCode
    {
        Completed = 0,
        Error = 1,
        New  = 2,
        InProgress = 3
    }
    class Server
    {
        public delegate void MessageSender(string message);
        public event MessageSender NewMessage;
        public event MessageSender NewStatus;
        public string _Info = String.Empty;
        private int port = 3257;
        public Dictionary<string, string[]> responseLog = new Dictionary<string, string[]>();
        public void Start()
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                NewStatus?.Invoke("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    NewStatus?.Invoke("Обработка команды...");
                    NewMessage?.Invoke(builder.ToString());
                    // отправляем ответ
                    string message = _Info;
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                NewStatus?.Invoke($"[SERVER ERROR] {ex.Message}");
            }
        }
        public string Info
        {
            set
            {
                _Info = value;
            }
        }
    }
}