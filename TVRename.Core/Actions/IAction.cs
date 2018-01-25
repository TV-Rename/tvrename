using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Core.Actions
{
    public interface IAction
    {
        string Type { get; }

        string Produces { get; }
        
        Task Run(CancellationToken ct);
    }
}
