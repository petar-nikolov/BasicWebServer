using HttpWebServer;
using HttpWebServer.Common;
using HttpWebServer.HTTP;
using HttpWebServer.Responses;

Console.WriteLine("The world begin here...");

static void AddFormDataAction(Request request, Response response)
{
    response.Body = string.Empty;

    foreach(var (key, value) in request.Form)
    {
        response.Body += $"{key} - {value}";
        response.Body += Environment.NewLine;
    }
}

var server = new HttpServer(routes => routes
.MapGet("/", new TextResponse("Hello from the server!"))
.MapGet("/HTML", new HtmlResponse(Constants.HtmlForm))
.MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
.MapPost("/HTML", new TextResponse(string.Empty, AddFormDataAction)));

server.Start();


