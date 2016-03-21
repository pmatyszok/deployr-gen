using System;

namespace deployRGen
{
    internal class GeneratedTypeDescription
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValueText { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(DefaultValueText);
        }

        public string CsType
        {
            get
            {
                switch (Type)
                {
                    case "bool" :
                        return Type;
                    case "numeric":
                        return "double";
                    case "character":
                        return "string";
                    case "date":
                    case "datetime":
                        return "DateTime";
                    case "data.frame":
                        return "RDataFrame";
                    default:
                        throw new NotImplementedException();

                }
            }
        }
    }
}