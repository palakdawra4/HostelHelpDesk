using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class WorkerService
    {
        private readonly HostelComplaintsDB _dbContext;

        public WorkerService(HostelComplaintsDB dbContext)
        {
            _dbContext = dbContext;
        }

        private Worker GetAvailableWorkerForTimeslot(int timeslotId)
        {
            var timeslot = _dbContext.TimeSlots.Find(timeslotId);
            if (timeslot == null)
            {
                // Handle invalid timeslot ID
                return null;
            }

            // Get the current date
            var currentDate = DateTime.Today;

            // Get the start and end time of the timeslot
            var startTime = timeslot.StartTime;
            var endTime = timeslot.EndTime;

            // Count the number of complaints for the given timeslot on the current date
            var numComplaintsForTimeslot = _dbContext.Complaints
                .Count(c => c.TimeSlotId == timeslotId &&
                            c.Created.Date == currentDate &&
                            c.WorkerId != null); // Exclude complaints already assigned to a worker

            // If the number of complaints for the timeslot is less than 2, find an available worker
            if (numComplaintsForTimeslot < 2)
            {
                // Query workers who are not assigned to two complaints for the same timeslot on the current date
                var availableWorkers = _dbContext.Workers
                    .Where(w => !_dbContext.Complaints
                        .Any(c => c.TimeSlotId == timeslotId &&
                                  c.Created.Date == currentDate &&
                                  c.WorkerId == w.Id))
                    .ToList();

                // Return the first available worker
                return availableWorkers.FirstOrDefault();
            }
            else
            {
                // Handle case when maximum limit of complaints for the timeslot is reached
                return null;
            }
        }

    }
}
