﻿using EventsWebApp.Application.Filters;
using EventsWebApp.Application.Interfaces;
using EventsWebApp.Domain.Enums;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.PaginationHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace EventsWebApp.Infrastructure.Repositories
{
    public class SocialEventRepository : ISocialEventRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SocialEventRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<SocialEvent> GetById(Guid id)
        {
            var socialEvent = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).FirstOrDefaultAsync(x => x.Id == id);

            return socialEvent;
        }

        public async Task<List<SocialEvent>> GetByName(string name)
        {
            var socialEvents = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).Where(x => x.EventName.Contains(name)).AsNoTracking().ToListAsync();

            return socialEvents;
        }

        public async Task<List<SocialEvent>> GetByDate(DateTime date)
        {
            var socialEvents = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).Where(x => x.Date.Date == date.Date).AsNoTracking().ToListAsync();

            return socialEvents;
        }

        public async Task<List<SocialEvent>> GetByCategory(E_SocialEventCategory category)
        {
            var socialEvents = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).Where(x => x.Category == category).AsNoTracking().ToListAsync();

            return socialEvents;
        }

        public async Task<List<SocialEvent>> GetByPlace(string place)
        {
            var socialEvents = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).Where(x => x.Place.Contains(place)).AsNoTracking().ToListAsync();

            return socialEvents;
        }

        public async Task<PaginatedList<SocialEvent>> GetSocialEvents(AppliedFilters filters, int pageIndex, int pageSize)
        {
            var a = filters.Date.IsNullOrEmpty();
            var b = filters.Category.IsNullOrEmpty();
            var allEvents = await _dbContext.SocialEvents
                                                    .Include(s => s.ListOfAttendees)
                                                    .AsNoTracking()
                                                    .Where(s => s.EventName.Contains(filters.Name ?? ""))
                                                    .Where(s => filters.Date.IsNullOrEmpty() || s.Date == DateTime.Parse(filters.Date).Date)
                                                    .Where(s => filters.Category.IsNullOrEmpty() || s.Category == (E_SocialEventCategory)Enum.Parse(typeof(E_SocialEventCategory), filters.Category))
                                                    .Where(s => s.Place.Contains(filters.Place ?? ""))
                                                    .OrderBy(s => s.EventName).ToListAsync();
            
            var onPageEvents = allEvents.Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling(allEvents.Count / (double)pageSize);

            return new PaginatedList<SocialEvent>(onPageEvents, pageIndex, totalPages);
        }

        public async Task<List<Attendee>> GetAllAttendeesByEventId(Guid id)
        {
            var socialEvent = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).FirstOrDefaultAsync(x => x.Id == id);
            if (socialEvent == null)
            {
                throw new Exception("No event was found");
            }
            return socialEvent.ListOfAttendees;
        }

        //REFACTOR
        public async Task<Attendee> GetAttendeeById(Guid socialEventId, Guid attendeeId)
        {
            var socialEvent = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).FirstOrDefaultAsync(x => x.Id == socialEventId);
            if (socialEvent == null)
            {
                throw new Exception("No event was found");
            }
            var attendeeList = socialEvent.ListOfAttendees;
            if(attendeeList == null)
            {
                throw new Exception("Attendee list is empty");
            }
            var attendee = attendeeList.FirstOrDefault(a => a.Id == attendeeId);

            return attendee;
        }

        public async Task<Attendee> GetAttendeeByEmail(Guid socialEventId, string attendeeEmail)
        {
            var socialEvent = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).FirstOrDefaultAsync(x => x.Id == socialEventId);
            if (socialEvent == null)
            {
                throw new Exception("No event was found");
            }
            var attendeeList = socialEvent.ListOfAttendees;
            if (attendeeList == null)
            {
                throw new Exception("Attendee list is empty");
            }
            var attendee = attendeeList.FirstOrDefault(a => a.Email == attendeeEmail);

            return attendee;
        }

        public async Task<Guid> Add(SocialEvent socialEvent)
        {
            await _dbContext.SocialEvents.AddAsync(socialEvent);

            return socialEvent.Id;
        }

        public async Task<Guid> Update(SocialEvent socialEvent)
        {
            await _dbContext.SocialEvents
                .Where(x => x.Id == socialEvent.Id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(s => s.EventName, s => socialEvent.EventName)
                    .SetProperty(s => s.Description, s => socialEvent.Description)
                    .SetProperty(s => s.Date, s => socialEvent.Date)
                    .SetProperty(s => s.Place, s => socialEvent.Place)
                    .SetProperty(s => s.MaxAttendee, s => socialEvent.MaxAttendee)
                    .SetProperty(s => s.Category, s => socialEvent.Category)
                    .SetProperty(s => s.Image, s => socialEvent.Image)
                    );
            return socialEvent.Id;
        }

        public async Task<Guid> AddAttendee(Guid socialEventId, Attendee attendee)
        {
            var socialEvent = await _dbContext.SocialEvents.Include(s => s.ListOfAttendees).FirstOrDefaultAsync(x => x.Id == socialEventId);

            if (socialEvent == null)
            {
                throw new Exception("No event was found");
            }

            var attendeesList = socialEvent.ListOfAttendees;
            if (attendeesList != null && attendeesList.Count + 1 > socialEvent.MaxAttendee)
            {
                throw new Exception("Max attendee number reached");
            }

            if (attendeesList == null)
            {
                attendeesList = new List<Attendee>();
            }
            attendeesList.Add(attendee);

            return socialEventId;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.SocialEvents.Where(x => x.Id == id).ExecuteDeleteAsync();

            return id;
        }
    }
}
