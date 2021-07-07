using CoreWars.Common;

namespace CoreWars.Competition
{
    public class CodeTemplate : IPythonCodeTemplate
    {
        public CodeTemplate(string template)
        {
            Template = template;
        }

        public string Template { get; }
    }
}