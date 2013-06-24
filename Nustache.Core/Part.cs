using System.Linq.Expressions;
using System.Text;
namespace Nustache.Core
{
    public abstract class Part
    {
        public abstract void Render(RenderContext context);

        public abstract Expression Compile<T>(StringBuilder builder) where T : class;

        public abstract string Source();
    }
}