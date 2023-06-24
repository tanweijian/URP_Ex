namespace UnityEngine.UI
{
    public class DisplayNameAttr: PropertyAttribute
    {
        private readonly string name = "";

        public string Name => name;

        public DisplayNameAttr(string name)
        {
            this.name = name;
        }
    }
}