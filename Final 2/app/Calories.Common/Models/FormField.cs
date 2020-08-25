using System.ComponentModel;

namespace Calories.Common.Models
{
    public class FormField
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public FormFieldOption[] Options { get; set; }

        public string Pattern { get; set; }

        public bool Required { get; set; }

        public bool Secret { get; set; }

        [DefaultValue(DefaultType)]
        public string Type { get; set; } = DefaultType;
        public const string DefaultType = "string";

        public int? MinLength { get; set; }

        public int? MaxLength { get; set; }

        public object Value { get; set; }
    }
}