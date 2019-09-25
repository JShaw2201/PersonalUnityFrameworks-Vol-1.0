
namespace JXFrame.Entity
{
    public interface IEntityBehavior
    {
        int Order { get; }

        void OnStart();

        void OnDispose();
    }
}
