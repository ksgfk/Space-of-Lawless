namespace KSGFK
{
    public interface IJobTimingTask : IJobCallback<JobTimingTask>
    {
        void RunTask();
    }
}