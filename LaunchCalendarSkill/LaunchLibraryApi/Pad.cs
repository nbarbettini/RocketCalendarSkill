using System.Collections.Generic;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public sealed class Pad
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InfoUrl { get; set; }
        public string WikiUrl { get; set; }
        public string MapUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<Agency> Agencies { get; set; }
    }
}
