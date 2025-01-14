using EventsWebApp.Domain.Models;

namespace EventsWebApp.Application.Dto
{
    public record SocialEventDto
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; }
        public string Place { get; set; } = string.Empty;
        public string Category { get; set; }
        public int MaxAttendee { get; set; }
        public List<Attendee> ListOfAttendees { get; set; } = [];
        public string Image { get; set; }
        public bool IsAlreadyInList { get; set; } = false;

        public SocialEventDto() { }

        public SocialEventDto(Guid id, string name,
                                            string description,
                                            string place,
                                            string date,
                                            string category,
                                            int maxAttendee,
                                            List<Attendee> attendees,
                                            string image)
        {
            Id = id;
            EventName = name;
            Description = description;
            Place = place;
            Date = date;
            Category = category;
            MaxAttendee = maxAttendee;
            ListOfAttendees = attendees;
            Image = image;

        }
    }
}