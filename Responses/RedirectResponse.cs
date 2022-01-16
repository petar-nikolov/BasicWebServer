using HttpWebServer.HTTP;

namespace HttpWebServer.Responses
{
    public class RedirectResponse : Response
    {
        public RedirectResponse(string location) : base(StatusCode.Found)
        {
            Headers.Add(Header.Location, location);
        }
    }
}
