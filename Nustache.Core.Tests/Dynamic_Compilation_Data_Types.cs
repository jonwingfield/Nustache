using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Dynamic_Compilation_Data_Types
    {
        class TypeTestClass
        {
            public string fieldName;

            public TypeTestClass() { }
            public TypeTestClass(string fieldName)
            {
                this.fieldName = fieldName;
            }
            public string Method() { return "returned from method"; }

            public Dictionary<string, int> ADictionary { get; set; }
            public Dictionary<string, TestObject> ComplexDictionary { get; set; }
            public Func<string, Object> Lambda { get; set; }
        }

        [Test]
        public void Methods()
        {
            var template = Template("A template with {{Method}}");
            var compiled = template.Compile<TypeTestClass>(null);
            var result = compiled(new TypeTestClass());
            Assert.AreEqual("A template with returned from method", result);            
        }

        [Test]
        public void Fields()
        {
            var template = Template("Showing {{fieldName}} values");
            var compiled = template.Compile<TypeTestClass>(null);
            var result = compiled(new TypeTestClass(fieldName: "this is private"));
            Assert.AreEqual("Showing this is private values", result);            
        }

        [Test]
        public void GenericDictionaries()
        {
            var template = Template("Showing {{ADictionary.key1}} values and {{ADictionary.value1}}");
            var compiled = template.Compile<TypeTestClass>(null);
            var result = compiled(new TypeTestClass
            {
                ADictionary = new Dictionary<string, int>
                {
                    { "key1", 10 },
                    { "value1", 15 }
                }
            });
            Assert.AreEqual("Showing 10 values and 15", result);            
        }

        [Test]
        public void EnumeratingGenericDictionaries()
        {
            Assert.Inconclusive("Not implemented");
        }

        [Test]
        public void GenericDictionariesAsSections()
        {
            var template = Template("Showing {{#ADictionary}} {{key1}} {{value1}}{{/ADictionary}}");

            Assert.Inconclusive("Not implemented");

            var compiled = template.Compile<TypeTestClass>(null);
            var result = compiled(new TypeTestClass
            {
                ADictionary = new Dictionary<string, int>
                {
                    { "key1", 10 },
                    { "value1", 15 }
                }
            });
            Assert.AreEqual("Showing  10 15", result);
        }

        [Test]
        public void DictionariesWithComplexTypesAsValues()
        {
            var template = Template("Showing {{ComplexDictionary.key1.TestString}}");
            var compiled = template.Compile<TypeTestClass>(null);
            var result = compiled(new TypeTestClass
            {
                ComplexDictionary = new Dictionary<string,TestObject> 
                {
                    { "key1", new TestObject { TestString = "testing" } },
                }
            });
            Assert.AreEqual("Showing testing", result);
        }

        [Test]
        public void GenericDictionary_KeyNotFound()
        {
            var template = Template("Showing {{ADictionary.key2}} values");
            var compiled = template.Compile<TypeTestClass>(null);

            var result = compiled(new TypeTestClass
            {
                ADictionary = new Dictionary<string, int>
                {
                    { "key1", 10 },
                    { "value1", 15 }
                }
            });

            Assert.Inconclusive("For integer types this is returning default value instead of empty string");
            Assert.AreEqual("Showing 0 values", result);
        }

        [Test]
        public void Lambdas()
        {
            var template = Template("{{#Lambda}}{{name}} is awesome.{{/Lambda}}");
            var compiled = template.Compile<TypeTestClass>(null);
            var result = compiled(new TypeTestClass { Lambda = (text) => string.Format("<b>{0}</b>", text) });
            Assert.AreEqual("<b>{{name}} is awesome.</b>", result);   
        }

        private Func<T, string> Compiled<T>(string text) where T : class
        {
            return Template(text).Compile<T>(null);
        }

        private Template Template(string text)
        {
            var template = new Template();
            template.Load(new StringReader(text));
            return template;
        }
    }
}
