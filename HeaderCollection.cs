namespace HttpWebServer
{
    public class HeaderCollection
    {
        private readonly IDictionary<string, Header> _headers = new Dictionary<string, Header>();

        public int Count => _headers.Count;

        public void Add(string name, string value)
        {
            _headers.Add(name, new Header(name, value));
        }
    }
}
