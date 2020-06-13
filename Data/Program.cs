using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CoreLibrary;
using Newtonsoft.Json;

namespace Data
{
    class Program
    {
        private static int port = 8086;
        private static IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        private static Socket serverSideSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        static void Main(string[] args)
        {
            try
            {
                InitializeData();

                StartPortListening();

                while (true)
                {
                    Socket handler = serverSideSocket.Accept();

                    var inputData = Decoder(handler);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + inputData);
                    BaseInputModel userInput = JsonConvert.DeserializeObject<BaseInputModel>(inputData);

                    var matrixA = GetMatrix(userInput.MatrixLeftName, userInput.RowBegin, userInput.RowEnd, userInput.ColumnBegin, userInput.ColumnEnd);
                    var matrixB = GetMatrix(userInput.MatrixRightName, userInput.RowBegin, userInput.RowEnd, userInput.ColumnBegin, userInput.ColumnEnd);

                    List<List<List<int>>> matrixes = new List<List<List<int>>>() {
                        matrixA,
                        matrixB
                    };

                    SendData(matrixes, handler);

                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            finally
            {
                CloseConnection();
            }
        }

        private static void CloseConnection()
        {
            serverSideSocket.Shutdown(SocketShutdown.Both);
            serverSideSocket.Close();
        }

        private static void SendData(List<List<List<int>>> matrixes, Socket handler)
        {
            string result = JsonConvert.SerializeObject(matrixes);

            byte[] sendData = Encoding.Unicode.GetBytes(result);

            handler.Send(sendData);
        }

        private static string Decoder(Socket handler)
        {
            string retval = "";
            int bytes = 0;
            byte[] data = new byte[512];

            do
            {
                bytes = handler.Receive(data);
                retval += Encoding.Unicode.GetString(data, 0, bytes);
            } while (handler.Available > 0);

            return retval;
        }

        private static void StartPortListening()
        {
            serverSideSocket.Bind(endPoint);
            serverSideSocket.Listen(10);
        }

        private static readonly Random random = new Random();
        private static int[,] GenerateRandomMatrix()
        {
            int size = random.Next(0, 16);
            int[,] matrix = new int[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size;x++)
                {
                    matrix[y,x] = random.Next(-100, 100);
                }
            }
            return matrix;
        }
        
        private const string DataFolderName = "MatrixesData";

        private static string pathTDF;
        private static string PathToDataFolder {
            get {
                if (string.IsNullOrEmpty(pathTDF))
                {
                    pathTDF = Path.GetFullPath("../" + DataFolderName);
                }
                return pathTDF;
            }
        }

        private static void InitializeData(int numberOfFiles = 10)
        {
            if (!Directory.Exists(PathToDataFolder))
            {
                for (int i = 0; i < numberOfFiles; i++)
                {
                    var matrix = GenerateRandomMatrix();
                    File.WriteAllText(PathToDataFolder + "M" + i + ".json", JsonConvert.SerializeObject(matrix));
                }
            }
        }
        
        private static List<List<int>> GetMatrix(string name, int rowBegin, int rowEnd, int colBegin, int colEnd)
        {
            try
            {
                var fileContent = File.ReadAllText(PathToDataFolder + name + ".json");
                int[,] matrix = JsonConvert.DeserializeObject<int[,]>(fileContent);
                var result = new List<List<int>>();
                for (int i = rowBegin; i < rowEnd; i++)
                {
                    result.Add(new List<int>());
                    for (int y = colBegin; y < colEnd; y++)
                    {
                        result[i - rowBegin].Add(matrix[i, y]);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}