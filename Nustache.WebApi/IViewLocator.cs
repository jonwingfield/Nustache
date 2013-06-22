using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nustache.WebApi
{
    public interface IViewLocator
    {
        bool CanProcess(Type viewType);
        string Find(Type viewType);
    }
}
