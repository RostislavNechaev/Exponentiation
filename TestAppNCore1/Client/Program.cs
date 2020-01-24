using System;
using System.Net.Sockets;
using System.Text;

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
            WriteMes("Ожидаем иснтрукции");
            client = new TcpClient();

            try
            {
                Connect();
                string message = ReceiveMes();
                WriteMes(message);
                while (true)
                {
                    ReadLine();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }

        }
        //Прием сообщения
        static string ReceiveMes()
        {
            byte[] data = new byte[255];
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
        //Отправить значения
        static bool SendMes(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            return true;
        }
        //прочесть строчку и разбить на составляющие
        static bool ReadLine()
        {
            int num = 0;
            string step = null;
            string message = Console.ReadLine();
            object[] mes = new Object[2] { 0, 0 };
            mes = message.Split(' ');
            //int num = (int)mes[0];

            try
            {
                num = Convert.ToInt32(mes[0]);
                step = ((string)mes[1]).ToLower();
                if (!(Proverka(num, step, message)))
                {
                    Err();
                    return true;
                }
            }
            catch (Exception)
            {
                Err();
                return true;
            }
            return true;


        }

        //проверка числа на правильность ввода и выполнение операции с ним
        static bool Proverka(int num, string step, string mes)
        {
            if ((num >= 0) && (num <= 100))
            {
                if (!(Chet(num)))
                {
                    switch (step)
                    {
                        case "square":
                            return Result(num, 2);
                        case "cube":
                            return Result(num, 3);
                        default:
                            return false;
                    }

                }
                else
                {
                    string report;
                    SendMes(mes);
                    report = ReceiveMes();
                    if (report == "Error")
                    {
                        return false;
                    }
                    WriteMes(report.ToString());
                    return true;
                }
            }
            else
                return false;
        }

        //возведение в степень
        static bool Result(int chislo, int stepen)
        {
            int resultat = chislo;
            for (int x = 1; x < stepen; x++)
            {
                resultat *= chislo;
            }
            WriteMes(resultat.ToString());
            return true;
        }
        //ошибка ввода
        static void Err()
        {
            Console.WriteLine("Введено неверное значение");
            Console.WriteLine("Повторите ввод!");
            ReadLine();
        }
        //Проверка на четность
        static bool Chet(int a)
        {
            return (a % 2) == 0;
        }
        //вывод сообщение
        static void WriteMes(string message)
        {
            Console.WriteLine(message);
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
            Environment.Exit(0);
        }


    }
}