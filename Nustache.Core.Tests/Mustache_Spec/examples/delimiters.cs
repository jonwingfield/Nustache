﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JsonCSharpClassGenerator;

namespace Nustache.Core.Tests.Mustache_Spec.Examples.delimiters
{

    public class PairBehavior
    {
        public PairBehavior(JObject obj)
        {
            this.text = JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(obj, "text"));
        }

        public readonly string text;
    }

    public class SpecialCharacters
    {
        public SpecialCharacters(JObject obj)
        {
            this.text = JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(obj, "text"));
        }

        public readonly string text;
    }

    public class Sections
    {
        public Sections(JObject obj)
        {
            this.section = JsonClassHelper.ReadBoolean(JsonClassHelper.GetJToken<JValue>(obj, "section"));
            this.data = JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(obj, "data"));
        }

        public readonly bool section;
        public readonly string data;
    }

    public class InvertedSections
    {
        public InvertedSections(JObject obj)
        {
            this.section = JsonClassHelper.ReadBoolean(JsonClassHelper.GetJToken<JValue>(obj, "section"));
            this.data = JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(obj, "data"));
        }

        public readonly bool section;
        public readonly string data;
    }

    public class PartialInheritence
    {
        public PartialInheritence(JObject obj)
        {
            this.value = JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(obj, "value"));
        }

        public readonly string value;
    }

    public class PostPartialBehavior
    {
        public PostPartialBehavior(JObject obj)
        {
            this.value = JsonClassHelper.ReadString(JsonClassHelper.GetJToken<JValue>(obj, "value"));
        }

        public readonly string value;
    }

    public class delimiters
    {

        public delimiters(string json)
            : this(JObject.Parse(json))
        {
        }

        public delimiters(JObject obj)
        {
            this.PairBehavior = JsonClassHelper.ReadStronglyTypedObject<PairBehavior>(JsonClassHelper.GetJToken<JObject>(obj, "PairBehavior"));
            this.SpecialCharacters = JsonClassHelper.ReadStronglyTypedObject<SpecialCharacters>(JsonClassHelper.GetJToken<JObject>(obj, "SpecialCharacters"));
            this.Sections = JsonClassHelper.ReadStronglyTypedObject<Sections>(JsonClassHelper.GetJToken<JObject>(obj, "Sections"));
            this.InvertedSections = JsonClassHelper.ReadStronglyTypedObject<InvertedSections>(JsonClassHelper.GetJToken<JObject>(obj, "InvertedSections"));
            this.PartialInheritence = JsonClassHelper.ReadStronglyTypedObject<PartialInheritence>(JsonClassHelper.GetJToken<JObject>(obj, "PartialInheritence"));
            this.PostPartialBehavior = JsonClassHelper.ReadStronglyTypedObject<PostPartialBehavior>(JsonClassHelper.GetJToken<JObject>(obj, "PostPartialBehavior"));
        }

        public readonly PairBehavior PairBehavior;
        public readonly SpecialCharacters SpecialCharacters;
        public readonly Sections Sections;
        public readonly InvertedSections InvertedSections;
        public readonly PartialInheritence PartialInheritence;
        public readonly PostPartialBehavior PostPartialBehavior;
    }

}
