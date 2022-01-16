using HttpWebServer.Common;
using HttpWebServer.HTTP;
using System.Text;

namespace HttpWebServer.Responses
{
    public class ContentResponse : Response
    {
        public ContentResponse(string content, string contentType, Action<Request, Response> preRenderAction = null) : base(StatusCode.OK)
        {
            Guard.AgainstNull(content);
            Guard.AgainstNull(contentType);

            Headers.Add(Header.ContentType, contentType); 
            Body = content;
            PreRenderAction = preRenderAction;
        }

        public override string ToString()
        {
            if(Body != null)
            {
                var contentLength = Encoding.UTF8.GetByteCount(Body).ToString();
                Headers.Add(Header.ContentLength, contentLength);
            }

            return base.ToString();
        }
    }
}
