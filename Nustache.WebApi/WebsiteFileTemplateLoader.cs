using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nustache.WebApi
{
    public class WebsiteFileTemplateLoader : ITemplateLoader
    {
        private readonly string _viewLocationFormat;
        private readonly string root;
        
        /// <summary>
        /// Loads templates from the website folder structure. The default location is [VirtualDir]\Views\{name}.mustache.
        /// </summary>
        /// <param name="viewLocationFormat">Format string for finding the view. The Name of the model returned from 
        /// your ApiController will be used as the parameter to the format.</param>
        public WebsiteFileTemplateLoader(string viewLocationFormat = "\\Views\\{0}.mustache")
        {
            _viewLocationFormat = viewLocationFormat;

            root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
                        .Replace("file:\\", string.Empty)
                        .Replace("\\bin", string.Empty)
                        .Replace("\\Debug", string.Empty)
                        .Replace("\\Release", string.Empty)
                        .Replace("\\AcceptanceTests", string.Empty);
        }

        public bool CanLoad(Type viewType)
        {
            return CanLoad(NameFromType(viewType));
        }

        public StreamReader Load(Type viewType)
        {
            return Load(NameFromType(viewType));
        }

        private string NameFromType(Type viewType)
        {
            return viewType.Name;
        }

        private string PathTo(string viewTypeName)
        {
            return root + String.Format(_viewLocationFormat, viewTypeName);
        }

        public bool CanLoad(string viewTypeName)
        {
            return File.Exists(PathTo(viewTypeName));
        }

        public StreamReader Load(string viewTypeName)
        {
            return File.OpenText(PathTo(viewTypeName));
        }
    }
}
