using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nustache.WebApi
{
    public class WebsiteViewLocator : IViewLocator
    {
        private readonly string _viewLocationFormat;
        private readonly string root;

        /// <param name="viewLocationFormat">Format string for finding the view. The Name of the model returned from 
        /// your ApiController will be used as the parameter to the format.</param>
        public WebsiteViewLocator(string viewLocationFormat = "\\Views\\{0}.mustache")
        {
            _viewLocationFormat = viewLocationFormat;

            root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
                        .Replace("file:\\", string.Empty)
                        .Replace("\\bin", string.Empty)
                        .Replace("\\Debug", string.Empty)
                        .Replace("\\Release", string.Empty)
                        .Replace("\\AcceptanceTests", string.Empty);
        }

        public bool CanProcess(Type viewType)
        {
            return File.Exists(Find(viewType));
        }

        public string Find(Type viewType)
        {
            var path = root + String.Format(_viewLocationFormat, viewType.Name);

            return path;
        }
    }
}
