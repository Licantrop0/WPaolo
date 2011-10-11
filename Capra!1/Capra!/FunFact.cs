namespace Capra
{
    public class FunFact
    {
        public string Type { get; set; }
        public string Text { get; set; }

        public FunFact(string type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}
