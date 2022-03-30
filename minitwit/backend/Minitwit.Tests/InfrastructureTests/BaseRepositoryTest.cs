namespace Minitwit.Tests.InfrastructureTests;

public abstract class BaseRepositoryTest : IDisposable
{
    protected readonly IMiniTwitContext Context;

    protected BaseRepositoryTest()
    {
        var setup = new RepositorySetup();
        Context = setup.GetTwitContext();
    }
    
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            Context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing:true);
        GC.SuppressFinalize(this);
    }
}