using HttpWebServer;

Console.WriteLine("The world begin here...");

var server = new HttpServer("127.0.0.1", 8080);
server.Start();
