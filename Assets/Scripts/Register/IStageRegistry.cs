using System.Threading.Tasks;

namespace KSGFK
{
    public interface IStageRegistry
    {
        void AddToWaitRegister(object obj);

        Task PreProcessEntry();

        void RegisterAll();
    }
}