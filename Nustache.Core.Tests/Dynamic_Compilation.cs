using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

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
            var template = new Template();
            template.Load(new StringReader("This is plain text"));
            var compiled = template.Compile<DottedNamesTruthy>(null);
            Assert.AreEqual("This is plain text", compiled(null));
        }

        [Test]
        public void LiteralTemplateWithComment()
        {
            var template = new Template();
            template.Load(new StringReader("This is {{!comment}}plain text"));
            var compiled = template.Compile<DottedNamesTruthy>(null);
            Assert.AreEqual("This is plain text", compiled(null));
        }

        [Test]
        public void TemplateWithVariables()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{TestString}} and {{TestBool}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestString = "Hello", TestBool = true });
            Assert.AreEqual("A template with Hello and True", result);            
        }

        [Test]
        public void HtmlEscape()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{TestString}} and {{TestBool}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestString = "<Hello> \"", TestBool = true });
            Assert.AreEqual("A template with &lt;Hello&gt; &quot; and True", result);
        }

        [Test]
        public void NestedTemplates()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{#Sub}} {{SubText}} here {{/Sub}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { SubText = "Blah" } });
            Assert.AreEqual("A template with  Blah here ", result);
        }

        [Test]
        public void Null_Nested_Template()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{#Sub}} {{SubText}} here {{/Sub}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = null });
            Assert.AreEqual("A template with ", result);
        }

        [Test]
        public void Dotted_Variable_Names()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{Sub.SubText}} here"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { SubText = "Blah" } });
            Assert.AreEqual("A template with Blah here", result);
        }

        [Test]
        public void Boolean_Sections()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{#TestBool}}data here{{/TestBool}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestBool = false });
            Assert.AreEqual("A template with ", result);
        }

        [Test]
        public void Boolean_Nested_Sections()
        {
            var template = new Template();
            template.Load(new StringReader("A template with {{#Sub.TestBool}}data here{{/Sub.TestBool}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Sub = new SubObject { TestBool = false } });
            Assert.AreEqual("A template with ", result);
        }

        [Test]
        public void Enumerable_Sections()
        {
            var template = new Template();
            template.Load(new StringReader("A template with{{#Items}} {{SubText}} {{/Items}}"));
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
            var template = new Template();
            template.Load(new StringReader("A template with{{#Items}} {{SubText}} {{/Items}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { Items = null });
            Assert.AreEqual("A template with", result);
        }

        [Test]
        public void Multiple_Levels_of_Enumerable_Sections()
        {
            var template = new Template();
            template.Load(new StringReader(
                @"A template with

{{#Items}} 

{{SubText}} 
{{#Sub}}
{{TestBool}}{{SubText}}
    {{#Sub}}
{{SubText}}
    {{/Sub}}
{{/Sub}} 

{{/Items}}"));
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
            var template = new Template();
            template.Load(new StringReader("A template with {{TestString}} and {{TestBool}}"));
            var compiled = template.Compile<TestObject>(null);
            var result = compiled(new TestObject { TestString = null });
            Assert.AreEqual("A template with  and False", result);     
        }

        // TODOs:
        [Test]
        public void Missing_Properties()
        {

        }

        [Test]
        public void Missing_SubProperties()
        {

        }

        //[Test]
        //public void NestedSection()
        //{
        //    var test = SectionTests.First(x => x["name"].ToString() == "Dotted Names - Truthy");

        //    var template = new Template();
        //    template.Load(new StringReader(test["template"].ToString()));
        //    //template.Compile<DottedNamesTruthy>();
        //    var rendered = Nustache.Core.Render.StringToString(test["template"].ToString(), new DottedNamesTruthy
        //    {
        //        a = new DottedNamesTruthy.Inner1
        //        {
        //            b = new DottedNamesTruthy.Inner2
        //            {
        //                c = true
        //            }
        //        }
        //    });

        //    Assert.AreEqual(test["expected"].ToString(), rendered);
        //}


    }
}
