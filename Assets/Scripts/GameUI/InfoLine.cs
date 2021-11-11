namespace GameUI
{
    public struct InfoLine
    {
            
        public string title;
        public string content;

        public InfoLine(string title, string content)
        {
            this.title   = title;
            this.content = content;
        }

        public override string ToString()
        {
            return title + ":" + content;
        }
    }
}