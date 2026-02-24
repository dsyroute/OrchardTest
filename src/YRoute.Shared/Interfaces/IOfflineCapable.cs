namespace YRoute.Shared.Interfaces;

/// <summary>
/// Marker interface for services that can provide offline-capable behavior.
/// TODO: Implement caching/local storage strategy per platform.
/// </summary>
public interface IOfflineCapable
{
    Task SyncAsync(CancellationToken cancellationToken = default);
}
