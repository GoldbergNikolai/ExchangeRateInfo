using ChainResourceService.Storages.Interfaces;

namespace ChainResourceService.Storages
{
    public class MemoryStorage<T> : IReadOnlyStorage<T>, IReadWriteStorage<T>
    {
        #region Private Members

        private T _value;
        private int _expiresIn;
        private DateTime _expiration;

        #endregion


        #region Constructors

        public MemoryStorage(int expiresIn = 1)
        {
            _expiresIn = expiresIn;
        }

        #endregion


        #region Public Methods

        public async Task<T> ReadValue()
        {
            if (DateTime.UtcNow <= _expiration)
            {
                return _value;
            }

            return default(T);
        }

        public async Task WriteValue(T value)
        {
            _value = value;
            _expiration = DateTime.UtcNow.AddHours(_expiresIn);
        }

        #endregion
    }
}
