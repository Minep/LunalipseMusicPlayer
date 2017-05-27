using System;

namespace LunalipseAPI.Generic
{
    public class LunalipsePluginInfo: Attribute
    {
        public string Author, Description, Team, Name;
        public string Version;
        public LunalipsePluginInfo()
        {
            Author = "Anonymous";
            Description = "This is a plugin";
            Team = "";
            Name = "LXP Plugin";
            Version = "1.0.0.0";
        }
    }
}
