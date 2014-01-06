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
        private bool _enableLiveReload = false;
        private Dictionary<string, int> _templateHashes;

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

        public bool EnableLiveReload
        {
            get
            {
                return _enableLiveReload;
            }
            set
            {
                _enableLiveReload = true;
                _templateHashes = new Dictionary<string, int>();
            }
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
            var rendered = Transform(type, value);

            using (var templateWriter = new StreamWriter(writeStream))
            {
                templateWriter.Write(rendered);
            }
        }

        private Template Load(string name)
        {
            if (_templateLoader.CanLoad(name))
            {
                var template = new Template();
                using (var streamReader = _templateLoader.Load(name))
                {
                    template.Load(streamReader);
                }
                return template;
            }
            else return null;
        }

        public string Transform(Type type, object value)
        {
            string viewName = ViewNameFrom(type);
            Func<object, string> compiledTemplate = null;

            if (_templateCache.Contains(viewName) && ViewHasntChanged(viewName, type))
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
                compiledTemplate = template.Compile(type, Load);
                _templateCache[viewName] = compiledTemplate;

                if (_enableLiveReload)
                {
                    _templateHashes[viewName] = ReadTemplateAsString(type).GetHashCode();
                }
            }

            var rendered = compiledTemplate(value);
            return rendered;
        }

        private bool ViewHasntChanged(string viewName, Type type)
        {
            if (!_enableLiveReload) return true;

            if (!_templateHashes.ContainsKey(viewName)) return false;

            return ReadTemplateAsString(type).GetHashCode() == _templateHashes[viewName];
        }

        private string ReadTemplateAsString(Type type)
        {
            using (var streamReader = _templateLoader.Load(type))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
