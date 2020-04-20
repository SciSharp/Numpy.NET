using System;

namespace CodeMinion.Core.Attributes
{
    public class PartialClassTemplateAttribute : Attribute
    {
        public string ClassName { get; set; }
        public string MemberName { get; set; }

        public PartialClassTemplateAttribute(string class_name, string member_name)
        {
            ClassName = class_name;
            MemberName = member_name;
        }
    }
}