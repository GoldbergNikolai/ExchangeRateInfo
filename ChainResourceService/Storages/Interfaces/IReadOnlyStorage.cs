namespace ChainResourceService.Storages.Interfaces
{
    public interface IReadOnlyStorage<T>
    {
        public Task<T> ReadValue();
    }
}
