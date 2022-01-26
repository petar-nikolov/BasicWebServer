using HttpWebServer.HTTP;
using HttpWebServer.Routing;
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
        private readonly RoutingTable _routingTable;

        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            _ipAddress = IPAddress.Parse(ipAddress);
            _port = port;
            _tcpListener = new TcpListener(_ipAddress, _port);
            routingTableConfiguration(_routingTable = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable) : this("127.0.0.1", port, routingTable)
        {
        }

        public HttpServer(Action<IRoutingTable> routingTable) : this(8080, routingTable)
        {
        }

        public async Task Start()
        {
            _tcpListener.Start();
            Console.WriteLine($"Server is listening on port: {_port}");
            Console.WriteLine($"Listening for requests");

            while (true)
            {
                var connection = await _tcpListener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    var networkStream = connection.GetStream();
                    var requestText = await ReadRequestAsync(networkStream);
                    Console.WriteLine(requestText);
                    var request = Request.Parse(requestText);
                    var response = _routingTable.MatchRequest(request);

                    //Execute pre-render action for the response
                    if (response.PreRenderAction != null)
                    {
                        response.PreRenderAction(request, response);
                    }

                    AddSession(request, response);
                    await WriteResponseAsync(networkStream, response);
                    connection.Close();
                });
            }
        }

        private void AddSession(Request request, Response response)
        {
            var sessionExists = request.Session.ContainsKey(Session.SessionCookieDateKey);
            if (!sessionExists)
            {
                request.Session[Session.SessionCookieDateKey] = DateTime.Now.ToString();
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);
            }
        }

        private async Task WriteResponseAsync(NetworkStream networkStream, Response response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());
            await networkStream.WriteAsync(responseBytes);
        }

        private async Task<string> ReadRequestAsync(NetworkStream networkStream)
        {
            var buffer = new byte[1024];
            var requestBuilder = new StringBuilder();
            var bytesTotal = 0;

            do
            {
                var readBytes = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                bytesTotal += readBytes;

                if(bytesTotal > 10 * 2024)
                {
                    throw new InvalidOperationException("Request is too large");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, readBytes));
            }
            while (networkStream.DataAvailable);
            return requestBuilder.ToString();
        }
    }
}
