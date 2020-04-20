using Newtonsoft.Json.Linq;

namespace CodeMinion.Core.Models
{
    public class Argument
    {
        public bool IsNullable { get; set; }
        public bool IsValueType { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNamedArg { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Before sending to Python convert to the given C# Type, only then convert to Python type
        /// </summary>
        public string ConvertToSharpType { get; set; }

        public int Position { get; set; }
        public bool IsReturnValue { get; set; }
        public string Tag { get; set; }

        /// <summary>
        /// This default value is not a compile time constant, so it can not be used as a C# default value in
        /// the function declaration. So if the parameter is passed as null it will be initialized with the given value.
        /// </summary>
        public string DefaultIfNull { get; set; }

        public bool PassOnlyIfNotNull { get; set; }
        public bool Ignore { get; set; }

        public Argument Clone()
        {
            return JObject.FromObject(this).ToObject<Argument>();
        }

        public void SetNullableOptional(string type, string @default = null)
        {
            Type = type;
            IsNullable = true;
            IsNamedArg = true;
            DefaultValue = @default;
        }

        public void SetType(string type, string @default=null)
        {
            Type = type;
            DefaultValue = @default;
        }

        public void MakeMandatory()
        {
            DefaultValue = null;
            IsNullable = false;
        }
    }
}