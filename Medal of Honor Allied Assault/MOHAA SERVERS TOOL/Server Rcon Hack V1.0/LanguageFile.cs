using System.Collections.Generic;

namespace mohaa_server_tool
{
     public class LanguageFile
    {
        public Dictionary<string, string> main { get; set; }
        public Dictionary<string, string> settings { get; set; }
        public Dictionary<string, string> status { get; set; }
        public Dictionary<string, string> viewfiles { get; set; }
        public Dictionary<string, string> brute { get; set; }
    }
}
