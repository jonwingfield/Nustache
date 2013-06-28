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
    public class MustacheViewFormatter : BufferedMediaTypeFormatter
    {
        private readonly MemoryCache _templateCache = MemoryCache.Default;
        private readonly ITemplateLoader _templateLoader;

        /// <summary>
        /// Media Type Formatter for rendering Web Api models as HTML via mustache.
        /// 
        /// Caches templates in MemoryCache.Default once they've been loaded and parsed to improve performance.
        /// 
        /// Implement ITemplateLoader if you want more control over template loading.
        /// If you just want to choose a new path or file extension, use the WebsiteFileTemplateLoader,
        /// but specify a new viewLocationFormat.
        /// </summary>
        /// <param name="templateLoader">Indicates to the formatter which templates are available and loads them.</param>
        public MustacheViewFormatter(ITemplateLoader templateLoader = null)
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("applictaion/xhtml"));
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/xhtml+xml"));

            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));
            SupportedEncodings.Add(new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true));

            _templateLoader = templateLoader ?? new WebsiteFileTemplateLoader();
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return _templateCache.Contains(ViewNameFrom(type)) || 
                _templateLoader.CanLoad(type);
        }

        private string ViewNameFrom(Type type)
        {
            return type.Name;
        }

        public override void WriteToStream(Type type, object value, System.IO.Stream writeStream, System.Net.Http.HttpContent content)
        {
            string viewName = ViewNameFrom(type);
            Func<object, string> compiledTemplate = null;

            if (_templateCache.Contains(viewName))
            {
                compiledTemplate = (Func<object, string>)_templateCache[viewName];
            }
            else
            {
                var template = new Template();
                using (var streamReader = _templateLoader.Load(type))
                {
                    template.Load(streamReader);
                }
                compiledTemplate = template.Compile(type, null);
                _templateCache[viewName] = template;
            }

            using (var templateWriter = new StreamWriter(writeStream))
            {
                templateWriter.Write(compiledTemplate(value));
            }
        }
    }
}
