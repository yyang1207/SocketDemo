using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketDemo.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(System.Net.IPEndPoint.Parse("127.0.0.1:9001"));
            socket.Listen(0);

            Console.WriteLine("服务器已启动，开始监听9001端口...............");

            TaskFactory factory = new TaskFactory();
            while (true)
            {
                Socket client = socket.Accept();
                factory.StartNew(() => 
                {
                    Span<byte> buffer = new byte[1024].AsSpan();

                    while (true)
                    {
                        int len = client.Receive(buffer);

                        if (len > 0)
                        {
                            string receive = Encoding.UTF8.GetString(buffer.Slice(0,len));
                            //Console.WriteLine($"收到数据:{receive}");

                            //{
                            //    string sendstr = $"{receive}-->x,时间:{DateTime.Now}";
                            //    Span<byte> sendBuffer = Encoding.UTF8.GetBytes(sendstr).AsSpan();
                            //    client.Send(sendBuffer);
                            //}

                            {
                                if (receive.ToLower() == "close" || receive.ToLower() == "exit") { client.Close(); return; }

                                factory.StartNew(() =>
                                {
                                    string sendstr = $"{receive}-->x,时间:{DateTime.Now}";
                                    Span<byte> sendBuffer = Encoding.UTF8.GetBytes(sendstr).AsSpan();
                                    client.Send(sendBuffer);
                                    //Console.WriteLine($"{sendstr}");
                                });
                            }
                        }
                    }
                });
            }


            Console.ReadLine();
        }
    }
}
