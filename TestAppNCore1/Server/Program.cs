﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Server
{
    class Program
    {
        static AddConnection server;
        static Thread listenThr;
        static void Main(string[] args)
        {
            try
            {
                server = new AddConnection();
                listenThr = new Thread(new ThreadStart(server.Listen));
                listenThr.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class AddConnection
    {
        const int port = 8888;
        static TcpListener listener;
        static Thread clientThr;
        protected internal void Listen()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");
                while (true)
                {
                    TcpClient tcplient = listener.AcceptTcpClient();
                    Client client = new Client(tcplient);
                    clientThr = new Thread(new ThreadStart(client.Process));
                    clientThr.Start();
                    Console.WriteLine("Новый клиент");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
    public class Client
    {
        protected internal NetworkStream stream { get; private set; }
        TcpClient client;
        public Client(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {

            try
            {
                stream = client.GetStream();
                SendMes("Введите через пробел число от 0 до 100 и степень (cube/square)");
                while (true)
                {
                    string message = ReceiveMes();
                    Console.WriteLine("Конвертируемая запись: {0}", message);
                    SplitMes(message, out int num, out string step);
                    Stepen(num, step);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        //Расчет 
        protected internal void Stepen(int num, string step)
        {
            switch (step)
            {
                case "square":
                    Result(num, 2);
                    break;
                case "cube":
                    Result(num, 3);
                    break;
                default:
                    Err();
                    break;
            }
            //SendMes(num.ToString());
        }
        //Отправка сообщения
        protected internal void SendMes(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Отправленно сообщение:{0}", message);
        }
        //ошибка ввода
        protected internal void Err()
        {
            SendMes("Error");
        }
        //Вызводим в степень
        protected internal void Result(int chislo, int stepen)
        {
            int resultat = chislo;
            for (int x = 1; x < stepen; x++)
            {
                resultat *= chislo;
            }
            SendMes(resultat.ToString());
        }
        //Прием сообщения
        protected internal string ReceiveMes()
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
            return builder.ToString();

        }
        //Разбитие сообщения
        protected internal void SplitMes(string message, out int num, out string step)
        {
            num = 0;
            step = null;
            object[] mes = new Object[2] { 0, 0 };
            mes = message.Split(' ');
            //num = Convert.ToInt32(mes[0]);
            //if ((mes.Length) > 1)
            //{
            //    step = Convert.ToString(mes[1]);
            //}

            try
            {
                num = Convert.ToInt32(mes[0]);
                Console.WriteLine("Конвертируемое число: {0}", num);
                step = ((string)mes[1]).ToLower();
                Console.WriteLine("Конвертируемое запись: {0}", step);
            }
            catch (Exception)
            {
                Err();
            }

        }
        //Разрыв связи
        protected internal void Disconnect()
        {
            stream.Close();
            client.Close();
        }
    }
}
