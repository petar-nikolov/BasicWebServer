using HttpWebServer.HTTP;

namespace HttpWebServer.Responses
{
    internal class TextResponse : ContentResponse
    {
        public TextResponse(string content, Action<Request, Response> preRenderAction = null) : base(content, ContentType.PlainText, preRenderAction)
        {
        }
    }
}
