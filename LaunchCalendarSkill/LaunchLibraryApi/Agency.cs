using System.Collections.Generic;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public sealed class Agency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }
        public string CountryCode { get; set; }
        public int Type { get; set; }
        public string WikiUrl { get; set; }
        public List<string> InfoUrls { get; set; }
    }
}
