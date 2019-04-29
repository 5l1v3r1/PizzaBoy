namespace PizzaBoy
{
    public class webTask
    {
        internal bool retainCookies = false;
        internal bool exportCookies = false;
        internal bool outputSource = true;

        public string URL { get; internal set; }
        public object POSTData { get; internal set; }
    }
}