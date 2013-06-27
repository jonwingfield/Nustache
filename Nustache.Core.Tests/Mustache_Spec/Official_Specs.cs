using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests.Mustache_Spec
{
    [TestFixture]
    public class Official_Specs
    {
        private static MustacheSpec.MustacheTest[] SectionTests() { return GetSpecs("sections"); }

        [Test, TestCaseSource("SectionTests")]
        public void Sections(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] CommentTests() { return GetSpecs("comments"); }

        [Test, TestCaseSource("CommentTests")]
        public void Comments(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        // TODO: support changing delimiters
        //private static MustacheSpec.MustacheTest[] DelimiterTests() { return GetSpecs("delimiters"); }

        //[Test, TestCaseSource("DelimiterTests")]
        //public void Delimiters(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] InterpolationTests() { return GetSpecs("interpolation"); }

        [Test, TestCaseSource("InterpolationTests")]
        public void Interpolation(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] InvertedTests() { return GetSpecs("inverted"); }

        [Test, TestCaseSource("InvertedTests")]
        public void Inverted(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        private static MustacheSpec.MustacheTest[] PartialsTests() { return GetSpecs("partials"); }

        [Test, TestCaseSource("PartialsTests")]
        public void Partials(MustacheSpec.MustacheTest test) { RunMustacheSpecs(test); }

        //[Test]
        public void Generate()
        {
            Generate_Classes_from_JSON.Run();
        }

        private static void RunMustacheSpecs(MustacheSpec.MustacheTest test)
        {
            TemplateLocator testDataTemplateLocator = name =>            {
                if (test.Partials != null && test.Partials[name] != null)
                {
                    var template = new Template();
                    template.Load(new StringReader(test.Partials[name].ToString()));
                    return template;
                };

                return null;
            };

            var rendered = Render.StringToString(test.Template, test.Example, testDataTemplateLocator);
            Assert.AreEqual(test.Expected, rendered, "JSON object rendering failed for " + test.Description);

            rendered = Render.StringToString(test.Template, test.StronglyTypedExample, testDataTemplateLocator);
            Assert.AreEqual(test.Expected, rendered, "Strongly typed rendering failed for " + test.Description);
        }

        private static MustacheSpec.MustacheTest[] GetSpecs(string name)
        {
            var spec = new MustacheSpec(name);

            return spec.MustacheTests.ToArray();
        }

    }
}
