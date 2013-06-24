using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nustache.Core
{
    public class CompileContext
    {
        private readonly Type targetType;
        private readonly RenderContext renderContext;
        private readonly ParameterExpression dataParam;

        public CompileContext(Section section, Type targetType, ParameterExpression dataParam, TemplateLocator templateLocator)
        {
            this.targetType = targetType;
            this.renderContext = new RenderContext(section, null, null, templateLocator);
            this.dataParam = dataParam;
        }

        public Type TargetType { get { return targetType; } }

        internal Expression CompiledGetter(string name)
        {
            return ValueGetter.CompiledGetter(targetType, name, dataParam);
        }
    }
}
