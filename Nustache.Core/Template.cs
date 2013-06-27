using System;
using System.IO;
using System.Linq.Expressions;

namespace Nustache.Core
{
    public class Template : Section
    {
        public Template()
            : this("#template") // I'm not happy about this fake name.
        {
        }

        public Template(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Loads the template.
        /// </summary>
        /// <param name="reader">The object to read the template from.</param>
        /// <remarks>
        /// The <paramref name="reader" /> is read until it ends, but is not
        /// closed or disposed.
        /// </remarks>
        /// <exception cref="NustacheException">
        /// Thrown when the template contains a syntax error.
        /// </exception>
        public void Load(TextReader reader)
        {
            string template = reader.ReadToEnd();

            var scanner = new Scanner();
            var parser = new Parser();

            parser.Parse(this, scanner.Scan(template));
        }

        public Func<T, string> Compile<T>(TemplateLocator templateLocator) where T : class
        {
            var param = Expression.Parameter(typeof(T), "data");

            var context = new CompileContext(this, typeof(T), param, templateLocator);

            var expression = Compile(context);
            Console.WriteLine(expression.ToString());
            return (Expression.Lambda<Func<T, string>>(expression, param)).Compile();
        }

        public Func<object, string> Compile(Type compileFor, TemplateLocator templateLocator)
        {
            var param = Expression.Parameter(typeof(object), "data");

            var context = new CompileContext(this, compileFor, 
                Expression.Convert(param, compileFor), templateLocator);

            var expression = Compile(context);
            Console.WriteLine(expression.ToString());
            return (Expression.Lambda<Func<object, string>>(expression, param)).Compile();
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="data">The data to use to render the template.</param>
        /// <param name="writer">The object to write the output to.</param>
        /// <param name="templateLocator">The delegate to use to locate templates for inclusion.</param>
        /// <remarks>
        /// The <paramref name="writer" /> is flushed, but not closed or disposed.
        /// </remarks>
        public void Render(object data, TextWriter writer, TemplateLocator templateLocator)
        {
            var context = new RenderContext(this, data, writer, templateLocator);

            Render(context);

            writer.Flush();
        }
    }
}