using System;
using System.Net.Sockets;
using System.Text;


namespace CoreLibrary
{
    public static class SocketExtension
    {
        public static string ReceiveData(this Socket sourceData)
        {
            TimeSpan waitingForConnection = TimeSpan.FromSeconds(25);
            DateTime startDate = DateTime.Now;
            
            ConnectonWaiter(sourceData, startDate, waitingForConnection);

            string result = "";
            byte[] data = new byte[512];

            result = GetResult(sourceData, data, result);
            return result;
        }

        private static string GetResult(Socket sourceData, byte[] data, string result)
        {
            int bytes;
            do
            {
                bytes = sourceData.Receive(data);
                result += (Encoding.Unicode.GetString(data, 0, bytes));
            } while (sourceData.Available > 0);

            return result;
        }

        private static void ConnectonWaiter(Socket sourceData, DateTime startDate, TimeSpan waitingForConnection)
        {
            while (!sourceData.Connected)
            {
                if (Math.Abs(startDate.Ticks - DateTime.Now.Ticks) > waitingForConnection.Ticks)
                {
                    break;
                }
            }
        }
    }
}