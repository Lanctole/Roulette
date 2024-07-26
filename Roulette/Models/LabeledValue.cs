namespace Roulette.Models
{
    public class LabeledValue
    {
        public LabeledValue(string value, string label)
        {
            Value = value;
            Label = label;
        }
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
