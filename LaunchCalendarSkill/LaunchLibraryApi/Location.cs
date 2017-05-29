using System.Collections.Generic;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public sealed class Location
    {
        public List<Pad> Pads { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string InfoUrl { get; set; }
        public string WikiUrl { get; set; }
        public string CountryCode { get; set; }
    }
}
