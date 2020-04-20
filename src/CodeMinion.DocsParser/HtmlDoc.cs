using HtmlAgilityPack;

namespace CodeMinion.Parser
{
    public class HtmlDoc
    {
        public string Filename { get; set; }
        //public string Url { get; set; }
        public string Text { get; set; }
        public HtmlDocument Doc { get; set; }
    }
}
