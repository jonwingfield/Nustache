using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using Nustache.Core.Compilation;

namespace Nustache.Core
{
    public class CompileContext
    {
        private int IncludeLimit = 255;
        private int _includeLevel = 0;

        private readonly Type targetType;
        private readonly RenderContext renderContext;
        private readonly Stack<Expression> _targetObjectStack = new Stack<Expression>();
        private readonly Stack<Section> _sectionStack = new Stack<Section>();
        private readonly TemplateLocator templateLocator;

        public CompileContext(Section section, Type targetType, Expression dataParam, TemplateLocator templateLocator)
        {
            this.targetType = targetType;
            this.renderContext = new RenderContext(section, null, null, templateLocator);
            this.templateLocator = templateLocator;

            _targetObjectStack.Push(dataParam);
            _sectionStack.Push(section);
        }

        public Type TargetType { get { return targetType; } }

        internal Expression CompiledGetter(string path)
        {
            return GetInnerExpression(path);
        }

        internal Expression GetInnerExpressions(string path, Func<Expression, Expression> innerExpression)
        {
            var value = GetInnerExpression(path);

            if (value.Type == typeof(bool))
            {
                return Expression.Condition(
                    value,
                    innerExpression(value),
                    Expression.Constant(""));
            }
            else if (value.Type.GetInterface("IEnumerable") != null)
            {
                return CompoundExpression.Enumerator(
                    itemCallback: innerExpression, 
                    enumerable: value);
            }
            else
            {
                return innerExpression(value);
            }
            
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
                    var value = GetExpressionFromPath(targetObject.Type, path, targetObject);

                    if (value != null)
                        return value;
                }
            }

            throw new CompilationException("Could not find " + path, 
                String.Join(", ", _targetObjectStack.Select(obj => obj.Type.Name)), 0, 0);
        }

        private Expression GetExpressionFromPath(Type type, string path, Expression targetObject)
        {
            var value = ValueGetter.CompiledGetter(type, path, targetObject);

            if (value != null)
            {
                return value;
            }

            value = targetObject;

            if (path == ".")
                return Expression.Call(value,
                    value.Type.GetMethod("ToString", new Type[0]));

            var names = path.Split('.');

            foreach (var name in names)
            {
                var parent = value;
                value = ValueGetter.CompiledGetter(type, name, value);

                if (value == null)
                    return null;

                type = value.Type;
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

        internal Expression Include(string templateName)
        {
            if (_includeLevel >= IncludeLimit)
            {
                throw new NustacheException(
                    string.Format("You have reached the include limit of {0}. Are you trying to render infinitely recursive templates or data?", IncludeLimit));
            }

            _includeLevel++;

            Expression compiled = null;

            TemplateDefinition templateDefinition = GetTemplateDefinition(templateName);

            if (templateDefinition != null)
            {
                compiled = templateDefinition.Compile(this);
            }
            else if (templateLocator != null)
            {
                var template = templateLocator(templateName);

                if (template != null)
                {
                    compiled = template.Compile(this);
                }
            }

            _includeLevel--;

            return compiled;
        }

        private TemplateDefinition GetTemplateDefinition(string name)
        {
            foreach (var section in _sectionStack)
            {
                var templateDefinition = section.GetTemplateDefinition(name);

                if (templateDefinition != null)
                {
                    return templateDefinition;
                }
            }

            return null;
        }
    }
}
