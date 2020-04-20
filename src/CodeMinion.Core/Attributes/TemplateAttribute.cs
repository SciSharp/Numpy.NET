using System;

namespace CodeMinion.Core.Attributes
{
    
    public class TemplateAttribute : Attribute
    {
        public string ApiFunction { get; set; }

        public TemplateAttribute(string api_function)
        {
            ApiFunction = api_function;
        }
    }
}
