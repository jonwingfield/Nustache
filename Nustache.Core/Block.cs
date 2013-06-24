using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

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
            var expressions = new List<Expression>();

            foreach (var value in context.GetInnerExpressions(Name))
            {
                context.Push(this, value);

                expressions.Add(context.WrapExpression(value,
                    base.Compile(context)));

                context.Pop();
            }

            if (expressions.Count > 0)
                return base.Concat(expressions);
            
            return null;
        }

        public override string ToString()
        {
            return string.Format("Block(\"{0}\")", Name);
        }
    }
}