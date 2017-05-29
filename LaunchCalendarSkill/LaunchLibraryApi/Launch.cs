using System;
using System.Collections.Generic;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public sealed class Launch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WindowStart { get; set; }
        public string WindowEnd { get; set; }
        public string Net { get; set; }
        public int? WsStamp { get; set; }
        public int? WeStamp { get; set; }
        public int? NetStamp { get; set; }
        public string IsoStart { get; set; }
        public string IsoEnd { get; set; }
        public string IsoNet { get; set; }
        public int? Status { get; set; }
        public int? TbdTime { get; set; }
        public List<string> VidUrls { get; set; }
        public List<string> InfoUrls { get; set; }
        public string HoldReason { get; set; }
        public string FailReason { get; set; }
        public int? TbdDate { get; set; }
        public int? Probability { get; set; }
        public string Hashtag { get; set; }
        public Location Location { get; set; }
        public Rocket Rocket { get; set; }
        public List<Mission> Missions { get; set; }
    }
}
