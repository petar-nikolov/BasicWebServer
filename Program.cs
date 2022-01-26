using HttpWebServer;
using HttpWebServer.Common;
using HttpWebServer.HTTP;
using HttpWebServer.Responses;
using System.Text;
using System.Web;

Console.WriteLine("The world begin here...");

static void AddFormDataAction(Request request, Response response)
{
    response.Body = string.Empty;

    foreach (var (key, value) in request.Form)
    {
        response.Body += $"{key} - {value}";
        response.Body += Environment.NewLine;
    }
}

static void AddCookiesAction(Request request, Response response)
{
    var requestHasCookies = request.Cookies.Any(x => x.Name != Session.SessionCookieName);
    var bodyText = string.Empty;
    if (requestHasCookies)
    {
        var cookieText = new StringBuilder();
        cookieText.AppendLine("<h1>Cookies</h1>");

        cookieText.Append("<table border='1'><tr><th>Name</th><th>Value></th></tr>");

        foreach (var cookie in request.Cookies)
        {
            cookieText.Append("<tr>");
            cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
            cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
            cookieText.Append("</tr>");
        }

        cookieText.Append("</table>");
    }
    else
    {
        bodyText = "<h1>Cookies set!</h1>";
        response.Cookies.Add("My-Cookie", "My-Value");
        response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
    }

    response.Body = bodyText;
}

static async Task<string> DownloadWebSiteContent(string url)
{
    var httpClient = new HttpClient();
    using (httpClient)
    {
        var response = await httpClient.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();
        return html.Substring(0, 2000);
    }
}

static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
{
    var downloads = new List<Task<string>>();
    foreach (var url in urls)
    {
        downloads.Add(DownloadWebSiteContent(url));
    }

    var responses = await Task.WhenAll(downloads);
    var responsesString = string.Join(Environment.NewLine + new String('-', 100), responses);
    await File.WriteAllTextAsync(fileName, responsesString);
}

static void DisplaySesstionInfoAction(Request request, Response response)
{
    var sessionExists = request.Session.ContainsKey(Session.SessionCookieDateKey);
    var bodyText = string.Empty;

    if (sessionExists)
    {
        var currentDate = request.Session[Session.SessionCookieDateKey];
        bodyText = $"Stored date: {currentDate}";
    }
    else
    {
        bodyText = "Current date stored!";
    }

    response.Body = string.Empty;
    response.Body += bodyText;
}

static void LoginAction(Request request, Response response)
{
    request.Session.Clear();
    var bodyText = string.Empty;

    var usernameMatches = request.Form["Username"] == Constants.UserName;
    var passwordMatches = request.Form["Password"] == Constants.Password;

    if (usernameMatches && passwordMatches)
    {
        request.Session[Session.SessionUserKey] = "MyUserId";
        request.Cookies.Add(Session.SessionCookieName, request.Session.Id);
        bodyText = "<h3>Logged successfully!</h3>";
    }
    else
    {
        bodyText = Constants.LoginForm;
    }

    response.Body = string.Empty;
    response.Body += bodyText;
}

static void LogoutAction(Request request, Response response)
{
    request.Session.Clear();
    response.Body = string.Empty;
}

static void GetUserDataAction(Request request, Response response)
{
    if (request.Session.ContainsKey(Session.SessionUserKey))
    {
        response.Body = string.Empty;
        response.Body += "<h3>Currently logged-in user " + $"is with username '{Constants.UserName}'</h3>";
    }
    else
    {
        response.Body = string.Empty;
        response.Body += "<h3>You should first log in " + "- <a href='/Login'>Login</a></h3>";
    }
}

await DownloadSitesAsTextFile(Constants.FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

var server = new HttpServer(routes => routes
.MapGet("/", new TextResponse("Hello from the server!"))
.MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
.MapGet("/HTML", new HtmlResponse(Constants.HtmlForm))
.MapPost("/HTML", new TextResponse(string.Empty, AddFormDataAction))
.MapGet("/Content", new HtmlResponse(Constants.DownloadForm))
.MapPost("/Content", new TextFileResponse(Constants.FileName))
.MapGet("/Cookies", new HtmlResponse("Cookies", AddCookiesAction))
.MapGet("/Session", new TextResponse("", DisplaySesstionInfoAction))
.MapGet("/Login", new HtmlResponse(Constants.LoginForm))
.MapPost("/Login", new HtmlResponse("", LoginAction))
.MapGet("/Logout", new HtmlResponse("", LogoutAction))
.MapGet("/UserProfile", new HtmlResponse("", GetUserDataAction)));

await server.Start();


