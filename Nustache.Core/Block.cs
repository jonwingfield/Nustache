using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using Nustache.Core.Compilation;

namespace Nustache.Core
{
    public class Block : Section
    {
        public Block(string name, params Part[] parts)
            : base(name)
        {
            Load(parts);
        }

        public override void Render(RenderContext context)
        {
            var lambda = context.GetValue(Name) as Lambda;
            if (lambda != null)
            {
                context.Write(lambda(InnerSource()).ToString());
                return;
            }

            foreach (var value in context.GetValues(Name))
            {
                context.Push(this, value);

                base.Render(context);

                context.Pop();
            }
        }

        internal override System.Linq.Expressions.Expression Compile(CompileContext context)
        {
            return context.GetInnerExpressions(Name, value =>
            {
                context.Push(this, value);

                var expression = CompoundExpression.NullCheck(value, 
                    nullValue: "", 
                    returnIfNotNull: base.Compile(context));

                context.Pop();

                return expression;
            });
        }

        public override string ToString()
        {
            return string.Format("Block(\"{0}\")", Name);
        }
    }
}