﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Nustache.Core.Compilation;

namespace Nustache.Core.Tests
{
    public class TestObject
    {
        public string TestString { get; set; }
        public bool TestBool { get; set; }
        public SubObject Sub { get; set; }
        public List<SubObject> Items { get; set; }
    }

    public class SubObject
    {
        public string SubText { get; set; }
        public bool TestBool { get; set; }

        public SubObject Sub { get; set; }
    }

    [TestFixture]
    public class Dynamic_Compilation
    {
        [Test]
        public void LiteralTemplate()
        {
            var template = Template("This is plain text");
            var compiled = template.Compile<DottedNamesTruthy>(null);
            Assert.AreEqual("This is plain text", compiled(null));
        }

        [Test]
        public void LiteralTemplateWithComment()
        {
            var template = Template("This is {{!comment}}plain text");
            var compiled = template.Compile<DottedNamesTruthy>(null);
            Assert.AreEqual("This is plain text", compiled(null));
        }

        [Test]
        public void TemplateWithVariables()
        {
            var template = Template("A template with {{TestString}} and {{TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestString = "Hello", TestBool = true });
            Assert.AreEqual("A template with Hello and True", result);            
        }

        [Test]
        public void HtmlEscape()
        {
            var template = Template("A template with {{TestString}} and {{TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestString = "<Hello> \"", TestBool = true });
            Assert.AreEqual("A template with &lt;Hello&gt; &quot; and True", result);
        }

        [Test]
        public void NestedTemplates()
        {
            var template = Template("A template with {{#Sub}} {{SubText}} here {{/Sub}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { SubText = "Blah" } });
            Assert.AreEqual("A template with  Blah here ", result);
        }

        [Test]
        public void Null_Nested_Template()
        {
            var template = Template("A template with {{#Sub}} {{SubText}} here {{/Sub}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = null });
            Assert.AreEqual("A template with ", result);
        }

        [Test]
        public void Dotted_Variable_Names()
        {
            var template = Template("A template with {{Sub.SubText}} here");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { SubText = "Blah" } });
            Assert.AreEqual("A template with Blah here", result);
        }

        [Test]
        public void Boolean_Sections()
        {
            var template = Template("A template with {{#TestBool}}data here{{/TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestBool = false });
            Assert.AreEqual("A template with ", result);
        }

        [Test]
        public void Boolean_Nested_Sections()
        {
            var template = Template("A template with {{#Sub.TestBool}}data here{{/Sub.TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { TestBool = false } });
            Assert.AreEqual("A template with ", result);
        }

        [Test]
        public void Enumerable_Sections()
        {
            var template = Template("A template with{{#Items}} {{SubText}} {{/Items}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject
            {
                Items = new List<SubObject>
                {
                    new SubObject { SubText = "a" },
                    new SubObject { SubText = "b" },
                    new SubObject { SubText = "c" },
                }
            });
            Assert.AreEqual("A template with a  b  c ", result);
        }

        [Test]
        public void Null_Enumerable_Values()
        {
            var template = Template("A template with{{#Items}} {{SubText}} {{/Items}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Items = null });
            Assert.AreEqual("A template with", result);
        }

        [Test]
        public void Multiple_Levels_of_Enumerable_Sections()
        {
            var template = Template(
                @"A template with

{{#Items}} 

{{SubText}} 
{{#Sub}}
{{TestBool}}{{SubText}}
    {{#Sub}}
{{SubText}}
    {{/Sub}}
{{/Sub}} 

{{/Items}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject
            {
                Items = new List<SubObject>
                {
                    new SubObject { SubText = "a" },
                    new SubObject { Sub = new SubObject { 
                        SubText = "Blah",
                        TestBool = true,
                        Sub = new SubObject {
                            SubText = "Third",
                            TestBool = false,
                        }
                    } },
                    new SubObject { SubText = "c" },
                }
            });
            Assert.AreEqual("A template witha  TrueBlahThirdc ", result.Replace("\r\n", ""));
        }

        [Test]
        public void Null_Values()
        {
            var template = Template("A template with {{TestString}} and {{TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestString = null });
            Assert.AreEqual("A template with  and False", result);     
        }

        [Test]
        public void Inverted_Sections()
        {
            var template = Template("A template with {{#Sub}}Failed{{/Sub}}{{^Sub}}{{TestBool}}{{/Sub}} trailing");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = null, TestBool = true });
            Assert.AreEqual("A template with True trailing", result);     
        }

        [Test]
        public void Nested_Inverted_Sections()
        {
            var template = Template("{{^Sub.Sub.TestBool}}Not Here{{/Sub.Sub.TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { Sub = new SubObject { TestBool = false } } });
            Assert.AreEqual("Not Here", result);     
        }

        [Test]
        public void Combined_Inverted_Sections()
        {
            var template = Template("{{#Sub.Sub}}Here{{^TestBool}}, but is it?{{/TestBool}}{{/Sub.Sub}}");
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { Sub = new SubObject { TestBool = false } } });
            Assert.AreEqual("Here, but is it?", result);     
        }

        [Test]
        public void Partials()
        {
            var template = Template("{{>text}} after partial");
            var compiled = template.Compile<TestObject>(name =>
                name == "text" ? Template("{{TestString}} {{#Sub.TestBool}}I am in you{{/Sub.TestBool}}") : null);
            var result = compiled(new TestObject { TestString = "a", Sub = new SubObject { TestBool = true } });
            Assert.AreEqual("a I am in you after partial", result);   
        }

        [Test]
        public void Internal_Templates_AKA_Inline_Partials()
        {
            var template = Template(
@"{{<text}}in partial {{Sub.SubText}} {{/text}}

{{>text}} after partial
");

            var compiled = template.Compile<TestObject>(null);

            var result = compiled(new TestObject { Sub = new SubObject { SubText = "Byaaah" } });
            Assert.AreEqual("in partial Byaaah  after partial", result.Replace("\r\n", ""));
        }

        [Test]
        public void Missing_Partial()
        {
            var template = Template("{{>text}} after partial");
            var compiled = template.Compile<TestObject>(name => null);
            var result = compiled(null);
            Assert.AreEqual(" after partial", result);   
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

        [Test, ExpectedException(ExpectedException=typeof(CompilationException), 
            ExpectedMessage="Could not find TestString1\nOn object: TestObject")]
        public void Missing_Properties()
        {
            var template = Template("A template with {{TestString1}} and {{TestBool}}");
            var compiled = template.Compile<TestObject>(null);
            compiled(new TestObject { TestString = "Hello", TestBool = true });
        }

        [Test, ExpectedException(ExpectedException = typeof(CompilationException),
            ExpectedMessage = "Could not find Missing\nOn object: SubObject")]
        public void Missing_SubProperties()
        {
            var template = Template("A template with {{Sub.Missing}}");
            var compiled = template.Compile<TestObject>(null);
            compiled(new TestObject { Sub = new SubObject { SubText = "Blah" } });
        }
    }
}
