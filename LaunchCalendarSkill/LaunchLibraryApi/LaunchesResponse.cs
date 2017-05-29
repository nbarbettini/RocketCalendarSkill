using System.Collections.Generic;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public class LaunchesResponse : PagedResponse
    {
        public Launch[] Launches { get; set; }
    }
}
