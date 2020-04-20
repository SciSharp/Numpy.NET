using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace CodeMinion.Parser
{
    public static class HtmlNodeExtensions
    {
        public static IEnumerable<HtmlNode> DescendantsOfClass(this HtmlNode self, string tag, string @class)
        {
            return self.Descendants(tag).Where(x => x.Attributes["class"]?.Value == @class);
        }
    }
}
