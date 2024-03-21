using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeChoreRepository
    {
        Task AddHomeChoreBase(HomeChoreBase homeChoreBase, int homeId);
        Task<HomeChoreTask> CreateHomeChore(HomeChoreRequest homeChoreRequest);
        Task Save();
        Task<List<HomeChoreTask>> GetAll(int id);
        Task<List<TaskAssignment>> GetCalendar(int id);
        Task Delete(int id);
        Task<HomeChoreTask> Get(int id);
        Task<TaskAssignment> GetTaskAssigment(int id);
        Task UpdateTaskAssignment(TaskAssignment taskAssignment);
        Task Update(HomeChoreTask homeChoreTask);
        Task SetHomeChoreDates(TaskSchedule taskSchedule);
        Task AddTaskAssignment(TaskAssignment taskAssignment);
        Task<bool> CheckOrHomeChoreWasAssigned(int id);
        Task DeleteAssignedTasks(int id);
        Task DeleteNotAssignedTasks(int id);
    }
}
