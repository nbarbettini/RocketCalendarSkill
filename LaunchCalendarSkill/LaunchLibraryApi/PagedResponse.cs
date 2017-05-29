namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public abstract class PagedResponse
    {
        public int Total { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
