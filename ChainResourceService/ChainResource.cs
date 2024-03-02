using ChainResourceService.Storages.Interfaces;
using ChainResourceService.Storages;

namespace ChainResourceService
{
    public class ChainResource<T>
    {
        #region Private Members

        private readonly IReadOnlyStorage<T>[] _storages;

        #endregion


        #region Constructors

        public ChainResource(params IReadOnlyStorage<T>[] storages)
        {
            _storages = storages;
        }

        #endregion


        #region Public Methods

        public async Task<T> GetValue()
        {
            foreach (var readonlyStorage in _storages)
            {
                var value = await readonlyStorage.ReadValue();
                if (value != null)
                {
                    if (readonlyStorage is WebServiceStorage<T>) await UpdateWriteableStorages(value);

                    return value;
                }
            }

            return default(T);
        }

        #endregion


        #region Private Methods

        private async Task UpdateWriteableStorages(T value)
        {
            foreach (var storage in _storages)
            {
                var writableStorage = storage as IReadWriteStorage<T>;
                if (writableStorage is IReadWriteStorage<T>)
                {
                    await writableStorage.WriteValue(value);
                }
            }
        }

        #endregion
    }
}
