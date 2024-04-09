using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeChoreRepository
    {
        Task AddHomeChoreBase(HomeChoreBase homeChoreBase, int homeId);
        Task<HomeChoreTask> CreateHomeChore(HomeChoreRequest homeChoreRequest, int userId, int? homeId);
        Task Save();
        Task<List<HomeChoreTask>> GetAll(int id);
        Task<List<TaskAssignment>> GetCalendar(int id);
        Task<List<TaskAssignment>> GetHomeChoresUserCalendar(int id);
        Task<List<TaskAssignment>> GetAssignedTasks(int userId);
        Task RemoveTaskAssignment(int taskId);
        Task Delete(int id);
        Task<HomeChoreTask> Get(int id);
        Task<TaskAssignment> GetTaskAssigment(int id);
        Task<TaskAssignment> GetLastAssigmentTask(int id);
        Task UpdateTaskAssignment(TaskAssignment taskAssignment);
        Task<int> GetTotalPointsAssigned(int memberId);
        Task<List<TaskAssignment>> GetUnassignedTasks(int homeId);
        Task Update(HomeChoreTask homeChoreTask);
        Task SetHomeChoreDates(TaskSchedule taskSchedule);
        Task AddTaskAssignment(TaskAssignment taskAssignment);
        Task<bool> CheckOrHomeChoreWasAssigned(int id);
        Task DeleteAssignedTasks(int id);
        Task DeleteNotAssignedTasks(int id);
        Task<bool> VoteArtical(int taskId, int userId, int voteValue);
        Task<int> GetTotalVotes(int taskId);
        Task<List<TaskAssignment>> GetDoneTasks(DateTime date);
		Task<List<TaskAssignment>> GetDoneTaskAssigment(int id);
        Task<List<TaskAssignment>> GetDoneTaskAssigments(int userId);
    }
}
