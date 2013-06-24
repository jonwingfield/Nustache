using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace Nustache.Core
{
    public class CompileContext
    {
        private readonly Type targetType;
        private readonly RenderContext renderContext;
        private readonly ParameterExpression dataParam;
        private readonly Stack<Expression> _targetObjectStack = new Stack<Expression>();
        private readonly Stack<Section> _sectionStack = new Stack<Section>();

        public CompileContext(Section section, Type targetType, ParameterExpression dataParam, TemplateLocator templateLocator)
        {
            this.targetType = targetType;
            this.renderContext = new RenderContext(section, null, null, templateLocator);
            this.dataParam = dataParam;

            _targetObjectStack.Push(dataParam);
        }

        public Type TargetType { get { return targetType; } }

        internal Expression CompiledGetter(string path)
        {
            return GetInnerExpression(path);
        }

        internal IEnumerable<Expression> GetInnerExpressions(string path)
        {
            var value = GetInnerExpression(path);
            yield return value;

            
            //else if (value is string)
            //{
            //    if (!string.IsNullOrEmpty((string)value))
            //    {
            //        yield return value;
            //    }
            //}
            //else if (GenericIDictionaryUtil.IsInstanceOfGenericIDictionary(value))
            //{
            //    if ((value as IEnumerable).GetEnumerator().MoveNext())
            //    {
            //        yield return value;
            //    }
            //}
            //else if (value is IDictionary) // Dictionaries also implement IEnumerable
            //                               // so this has to be checked before it.
            //{
            //    if (((IDictionary)value).Count > 0)
            //    {
            //        yield return value;
            //    }
            //}
            //else if (value is IEnumerable)
            //{
            //    foreach (var item in ((IEnumerable)value))
            //    {
            //        yield return item;
            //    }
            //}
            //else
            //{
            //    yield return value;
            //}
        }

        private Expression GetInnerExpression(string path)
        {
            foreach (var targetObject in _targetObjectStack)
            {
                if (targetObject != null)
                {
                    var value = GetExpressionFromPath(targetObject.Type, path);

                    //if (!ReferenceEquals(value, ValueGetter.NoValue))
                    //{
                        return value;
                    //}
                }
            }

            return null;
        }

        private Expression GetExpressionFromPath(Type type, string path)
        {
            var value = ValueGetter.CompiledGetter(type, path, _targetObjectStack.Peek());

            if (value != null)
            {
                return value;
            }

            var names = path.Split('.');
            value = _targetObjectStack.Peek();

            foreach (var name in names)
            {
                value = ValueGetter.CompiledGetter(type, name, value);
                type = value.Type;

                if (value == null)
                {
                    break;
                }
            }

            return value;
        }

        internal void Push(Section section, Expression targetObjectRef)
        {
            _sectionStack.Push(section);
            _targetObjectStack.Push(targetObjectRef);
        }

        internal void Pop()
        {
            _sectionStack.Pop();
            _targetObjectStack.Pop();
        }

        internal Expression WrapExpression(Expression conditional, Expression inner)
        {
            if (conditional.Type == typeof(bool))
            {
                return Expression.Condition(
                    conditional,
                    inner,
                    Expression.Constant(""));
            }
            else return inner;
        }
    }
}
