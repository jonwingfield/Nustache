﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nustache.WebApi
{
    /// <summary>
    /// Handles the finding and loading of templates
    /// </summary>
    public interface ITemplateLoader
    {
        /// <summary>
        /// Indicates if the template exists and can be loaded
        /// </summary>
        /// <param name="viewType">The type of the model returned from an ApiController</param>
        bool CanLoad(Type viewType);
        
        /// <summary>
        /// Loads a template as a StreamReader.  Must not throw if CanLoad is true for this type.
        /// </summary>
        /// <param name="viewType">The type of the model returned from an ApiController</param>
        /// <returns>A StreamReader representation of the template</returns>
        StreamReader Load(Type viewType);
    }
}