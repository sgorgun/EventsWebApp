using System.ComponentModel.DataAnnotations;

namespace EventsWebApp.Server.Contracts
{
    public record CreateSocialEventRequest([Required] string EventName, 
                                            [Required] string Description, 
                                            [Required] string Place, 
                                            [Required] string Date, 
                                            [Required] string Category, 
                                            [Required] int MaxAttendee);
}