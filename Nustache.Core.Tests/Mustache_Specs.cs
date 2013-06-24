using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Nustache.Core.Tests
{
    public class DottedNamesTruthy
    {
        public Inner1 a { get; set; }

        public class Inner1
        {
            public Inner2 b { get; set; }
        }

        public class Inner2
        {
            public bool c { get; set; }
        }
    }

    [TestFixture]
    public class Mustache_Specs
    {
        private static readonly JToken[] SectionTests = GetSpecs("sections");

        [Test, TestCaseSource("SectionTests")]
        public void Sections(JToken test) { RunMustacheSpecs(test); }

        private static readonly JToken[] CommentTests = GetSpecs("comments");

        [Test, TestCaseSource("CommentTests")]
        public void Comments(JToken test) { RunMustacheSpecs(test); }

        private static readonly JToken[] DelimiterTests = GetSpecs("delimiters");

        [Test, TestCaseSource("DelimiterTests")]
        public void Delimiters(JToken test) { RunMustacheSpecs(test); }

        private static readonly JToken[] InterpolationTests = GetSpecs("interpolation");

        [Test, TestCaseSource("InterpolationTests")]
        public void Interpolation(JToken test) { RunMustacheSpecs(test); }
        
        private static readonly JToken[] InvertedTests = GetSpecs("inverted");

        [Test, TestCaseSource("InvertedTests")]
        public void Inverted(JToken test) { RunMustacheSpecs(test); }

        private static readonly JToken[] PartialsTests = GetSpecs("partials");

        [Test, TestCaseSource("PartialsTests")]
        public void Partials(JToken test) { RunMustacheSpecs(test); }

        private static void RunMustacheSpecs(JToken test)
        {
            var data = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(test["data"].ToString());

            var rendered = Nustache.Core.Render.StringToString(test["template"].ToString(), data, name =>
                {
                    if (test["partials"] != null && test["partials"][name] != null)
                    {
                        var template = new Template();
                        template.Load(new StringReader(test["partials"][name].ToString()));
                        return template;
                    };

                    return null;
                });

            Assert.AreEqual(test["expected"].ToString(), rendered, test["desc"].ToString());
        }

        private static JToken[] GetSpecs(string name)
        {
            var specs = File.ReadAllText(String.Format("../../spec/specs/{0}.json", name));

            var spec = JObject.Parse(specs);

            return spec["tests"].ToArray();
        }

    }
}
