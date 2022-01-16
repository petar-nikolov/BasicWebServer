﻿using HttpWebServer.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpWebServer.Responses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string content) : base(content, ContentType.Html)
        {
        }
    }
}
