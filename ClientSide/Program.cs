using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CoreLibrary;
using Newtonsoft.Json;

namespace ClientSide
{
    class Program
    {
        static int port = 8004;
        static string ipAddress = "127.0.0.1";
        private static string _rowBegin;
        private static string _rowEnd;
        private static string _columnBegin;
        private static string _columnEnd;
        private static string _matrixLeft;
        private static string _matrixRight;

        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ConnetSocket(socket);
                
                UserInput();

                var userInput = new BaseInputMode(int.Parse(_rowBegin),
                    int.Parse(_rowEnd),
                    int.Parse(_columnBegin),
                    int.Parse(_columnEnd),
                    _matrixLeft,
                    _matrixRight);

                string json = JsonConvert.SerializeObject(userInput);
                byte[] data = Encoding.Unicode.GetBytes(json);
                socket.Send(data);

                data = new byte[512];

                var decodedResult = Decoder(socket, data);

                Console.WriteLine(decodedResult);
                var result = JsonConvert.DeserializeObject<List<List<int>>>(decodedResult);
                OutputResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            Console.ReadKey();
        }

        private static void OutputResult(List<List<int>> result)
        {
            foreach (var row in result)
            {
                foreach (var cell in row)
                {
                    Console.Write(cell + " ");
                }

                Console.WriteLine();
            }
        }

        private static string Decoder(Socket socket, byte[] data)
        {
            string retval = "";
            int bytes;
            
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                retval += Encoding.Unicode.GetString(data, 0, bytes);
            } while (socket.Available > 0);

            return retval;
        }

        private static void UserInput()
        {
            Console.WriteLine("Input row begin");
            _rowBegin = Console.ReadLine();
            Console.WriteLine("Input row end");
            _rowEnd = Console.ReadLine();
            Console.WriteLine("Input column begin");
            _columnBegin = Console.ReadLine();
            Console.WriteLine("Input column end");
            _columnEnd = Console.ReadLine();
            Console.WriteLine("Input matrix left");
            _matrixLeft = Console.ReadLine();
            Console.WriteLine("Input matrix right");
            _matrixRight = Console.ReadLine();
        }

        private static void ConnetSocket(Socket socket)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            socket.Connect(ipPoint);
        }
    }
}