using System;
using System.Linq.Expressions;
using System.Text;

namespace Nustache.Core
{
    public class EndSection : Part
    {
        private readonly string _name;

        public EndSection(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public override void Render(RenderContext context)
        {
        }

        public override string Source()
        {
            return "";
        }

        public override string ToString()
        {
            return string.Format("EndSection(\"{0}\")", _name);
        }

        public override Expression Compile<T>(StringBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}