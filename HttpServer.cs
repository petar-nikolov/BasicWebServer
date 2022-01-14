using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpWebServer
{
    public class HttpServer
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly TcpListener _tcpListener;

        public HttpServer(string ipAddress, int port)
        {
            _ipAddress = IPAddress.Parse(ipAddress);
            _port = port;
            _tcpListener = new TcpListener(_ipAddress, _port);
        }

        public void Start()
        {
            _tcpListener.Start();
            Console.WriteLine($"Server is listening on port: {_port}");
            Console.WriteLine($"Listening for requests");

            while (true)
            {
                var connection = _tcpListener.AcceptTcpClient();
                var networkStream = connection.GetStream();
                WriteResponse(networkStream, "Hello World");
                connection.Close();
            }
        }

        private static void WriteResponse(NetworkStream networkStream, string content)
        {
            var contentLength = Encoding.UTF8.GetByteCount(content);
            var response = $@"HTTP/1.1 200 OK 
                            Content-Type: text/plain; charset=UTF-8
                            Content-Length: {contentLength}
                            
                            {content}";

            var responseBytes = Encoding.UTF8.GetBytes(response);
            networkStream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}
