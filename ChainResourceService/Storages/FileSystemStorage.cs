using ChainResourceService.Storages.Interfaces;
using System.Text.Json;

namespace ChainResourceService.Storages
{
    public class FileSystemStorage<T> : IReadOnlyStorage<T>, IReadWriteStorage<T>
    {
        #region Private Members

        private readonly string _filePath;
        private int _expiresIn;
        private DateTime _expiration;

        #endregion


        #region Constructors

        public FileSystemStorage(string filePath, int expiresIn = 4)
        {
            _filePath = filePath;
            _expiresIn = expiresIn;
        }

        #endregion


        #region Public Methods

        public async Task<T> ReadValue()
        {
            if (DateTime.UtcNow <= _expiration && File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<T>(json);
            }

            return default(T);
        }

        public async Task WriteValue(T value)
        {
            var json = JsonSerializer.Serialize(value);
            await File.WriteAllTextAsync(_filePath, json);
            _expiration = DateTime.UtcNow.AddHours(_expiresIn);
        }

        #endregion
    }
}
