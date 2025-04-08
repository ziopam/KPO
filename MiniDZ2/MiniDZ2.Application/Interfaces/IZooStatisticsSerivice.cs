namespace MiniDZ2.Application.Interfaces
{
    public interface IZooStatisticsSerivice
    {
        Task<int> GetTotalAnimalCountAsync();
        Task<int> GetTotalHealthyAnimalsAsync();
        Task<int> GetTotalSickAnimalsAsync();
        Task<int> GetTotalEnclosureCountAsync();
        Task<int> GetAvailableEnclosuresAsync();
        Task<int> GetTotalFeedingScheduleCountAsync();
    }
}
