namespace CodeMinion.Core.Models
{
    public class Property : Declaration
    {
        public bool HasSetter { get; set; } = true;

        // shortcut for the type of the first return value.
        // For a property that returns a tuple use Returns instead
        public string Type
        {
            get
            {
                if (Returns.Count == 0)
                    return null;
                return Returns[0].Type;
            }
            set
            {
                if (Returns.Count == 0)
                    Returns.Add(new Argument(){Type = value});
                Returns[0].Type=value;
            }
        }

        public string DefaultValue { get; set; }

    }
}