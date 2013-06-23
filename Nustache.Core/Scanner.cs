using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace Nustache.Core
{
    public class Scanner
    {
        private static readonly Regex _markerRegex = new Regex(@"\{\{([\{]?[^}]+?\}?)\}\}");
        private static readonly Regex _standaloneRegex = new Regex(@"(^|\r?\n)[\r\t\v ]*({\{\s*[#\/!\<\>^]+[^}]+?\}\})[\r\t\v ]*(\r?\n|$)");

        public IEnumerable<Part> Scan(string template)
        {
            int i = 0;
            Match m;

            template = _standaloneRegex.Replace(template, match => match.Groups[1].Value + match.Groups[2].Value);
            
            while (true)
            {
                if ((m = _markerRegex.Match(template, i)).Success)
                {
                    string literal = template.Substring(i, m.Index - i);

                    if (literal != "")
                    {
                        yield return new LiteralText(literal);
                    }

                    string marker = m.Groups[1].Value;

                    marker = marker.Trim();

                    if (marker[0] == '#')
                    {
                        yield return new Block(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '^')
                    {
                        yield return new InvertedBlock(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '<')
                    {
                        yield return new TemplateDefinition(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '/')
                    {
                        yield return new EndSection(marker.Substring(1).Trim());
                    }
                    else if (marker[0] == '>')
                    {
                        yield return new TemplateInclude(marker.Substring(1).Trim());
                    }
                    else if (marker[0] != '!')
                    {
                        yield return new VariableReference(marker.Trim());
                    }

                    i = m.Index + m.Length;

                    //Match s;
                    //if (standalone && (s = _stripRegex.Match(template, i)).Success)
                    //    i += s.Length;
                    //if (stripOutNewLine && template[i] == '\n')
                    //    i += 1;
                }
                else
                {
                    break;
                }
            }

            if (i < template.Length)
            {
                yield return new LiteralText(template.Substring(i));
            }
        }
    }
}