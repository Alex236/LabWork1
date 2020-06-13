using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace ServerSide
{
    class Program
    {
        private const int clientPort = 8004;
        private const int dataServerPort = 8086;
        private const string ipAdress = "127.0.0.1";

        private static readonly IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress), clientPort);
        private static readonly IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ipAdress), dataServerPort);

        private static readonly Socket clientSideSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly Socket serverSideSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            try
            {
                clientSideSocket.Bind(clientEndPoint);
                clientSideSocket.Listen(10);

                serverSideSocket.Connect(serverEndpoint);
                Console.WriteLine(serverSideSocket.Connected);
                
                while (true)
                {
                    Socket handler = clientSideSocket.Accept();

                    var decodedResult = Decoder(handler);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + decodedResult);
                    var userInput = Encoding.Unicode.GetBytes(decodedResult);

                    serverSideSocket.Send(userInput);

                    string serverData = ServerDecoder();

                    var initialData = JsonConvert.DeserializeObject<List<List<List<int>>>>(serverData);

                    var result = MultiplyMatrixes(initialData[0], initialData[1]);

                    string message = JsonConvert.SerializeObject(result);
                    SendData(message, handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnections();
                Console.Read();
            }
        }

        private static void SendData(string message, Socket handler)
        {
            byte[] data = new byte[256];
            data = Encoding.Unicode.GetBytes(message);
            handler.Send(data);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static string ServerDecoder()
        {
            var data = new byte[256];
            string retval = "";
            int bytes = 0;

            do
            {
                if (!serverSideSocket.Connected)
                {
                    serverSideSocket.Connect(serverEndpoint);
                }

                bytes = serverSideSocket.Receive(data, data.Length, 0);
                retval += Encoding.Unicode.GetString(data, 0, bytes);
            } while (serverSideSocket.Available > 0);

            return retval;
        }

        private static string Decoder(Socket handler)
        {
            string retval = "";
            int bytes = 0;
            byte[] data = new byte[256];

            do
            {
                bytes = handler.Receive(data);
                retval += Encoding.Unicode.GetString(data, 0, bytes);
            } while (handler.Available > 0);

            return retval;
        }

        private static void CloseConnections()
        {
            clientSideSocket.Shutdown(SocketShutdown.Both);
            clientSideSocket.Close();
            serverSideSocket.Shutdown(SocketShutdown.Both);
            serverSideSocket.Close();
        }

        private static List<List<int>> MultiplyMatrixes(List<List<int>> matrixA, List<List<int>> matrixB)
        {
            var res = new List<List<int>>(matrixA.Count);

            for (int i = 0; i < matrixA.Count; i++)
            {
                res.Add(new List<int>(matrixB[0].Count));
                for (int j = 0; j < matrixB[0].Count; j++)
                {
                    int sum = 0;
                    for (int z = 0; z < matrixB.Count; z++)
                    {
                        sum += matrixA[i][z] * matrixB[z][j];
                    }
                    res[i].Add(sum);
                }
            }
            return res;
        }
    }
}