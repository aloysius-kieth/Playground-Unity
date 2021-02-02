using System.Threading.Tasks;

public interface IManager
{
    int ExecutionPriority { get; set; }
    bool IsReady { get; set; }
    void Init();
}