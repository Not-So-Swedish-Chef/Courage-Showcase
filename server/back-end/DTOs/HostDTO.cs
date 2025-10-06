namespace back_end.DTOs
{
    public class HostDTO
    {
        public int Id { get; set; }
        public string? AgencyName { get; set; }
        public string? Bio { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public List<EventDTO>? Events { get; set; }
    }
}
