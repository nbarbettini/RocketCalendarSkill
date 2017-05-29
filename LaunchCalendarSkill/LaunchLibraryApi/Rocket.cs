using System.Collections.Generic;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public sealed class Rocket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Configuration { get; set; }
        public string FamilyName { get; set; }
        public List<Agency> Agencies { get; set; }
        public string WikiUrl { get; set; }
        public List<string> InfoUrls { get; set; }
        public List<int> ImageSizes { get; set; }
        public string ImageUrl { get; set; }
    }
}
