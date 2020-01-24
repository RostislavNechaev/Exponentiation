using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        const int port = 8888;
        const string address = "127.0.0.1";
        static TcpClient client;
        static NetworkStream stream;
        static void Main(string[] args)
        {
            Console.WriteLine("Ожидаем иснтрукции");
            client = new TcpClient();

            try
            {
                Connect();
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMes));
                receiveThread.Start();
                SendMes();
            }
            catch (Exception)
            {
                Console.WriteLine("Пользователь вышел");
                Disconnect();
            }

        }
        //Прием сообщения
        static void ReceiveMes()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; 
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); 
                    Console.ReadLine();
                    Disconnect();
                }
            }

        }
        //Отправить значения
        static bool SendMes()
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
   

        
        //Подключение
        static void Connect()
        {
            try
            {
                client.Connect(address, port);
            }
            catch (Exception)
            {
                Connect();
            }

            stream = client.GetStream();
        }
        //Разрыв соединения
        static void Disconnect()
        {
            
            client.Close();
            if (stream != null)
                stream.Close();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadLine();
            Environment.Exit(0);
        }


    }
}