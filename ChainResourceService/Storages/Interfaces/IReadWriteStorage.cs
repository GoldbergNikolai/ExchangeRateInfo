namespace ChainResourceService.Storages.Interfaces
{
    public interface IReadWriteStorage<T>
    {
        public Task WriteValue(T value);
    }
}
