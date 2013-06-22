using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Formatting;
using System.Runtime.Caching;
using Nustache.Core;
using System.IO;

namespace Nustache.WebApi
{
    public class NustacheViewFormatter : BufferedMediaTypeFormatter
    {
        private readonly MemoryCache _templateCache = MemoryCache.Default;
        private readonly IViewLocator _viewLocator;

        public NustacheViewFormatter(IViewLocator viewLocator = null)
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("applictaion/xhtml"));
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/xhtml+xml"));

            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));
            SupportedEncodings.Add(new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true));

            _viewLocator = viewLocator ?? new WebsiteViewLocator();
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return _templateCache.Contains(ViewNameFrom(type)) || 
                _viewLocator.CanProcess(type);
        }

        private string ViewNameFrom(Type type)
        {
            return type.Name;
        }

        public override void WriteToStream(Type type, object value, System.IO.Stream writeStream, System.Net.Http.HttpContent content)
        {
            string viewName = ViewNameFrom(type);
            Template template = null;

            if (_templateCache.Contains(viewName))
            {
                template = (Template)_templateCache[viewName];
            }
            else
            {
                template = new Template();
                template.Load(File.OpenText(_viewLocator.Find(type)));
                _templateCache[viewName] = template;
            }

            template.Render(value, new StreamWriter(writeStream), null);
        }
    }
}
