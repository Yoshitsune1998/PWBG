using System.Collections.Generic;
using System;
namespace PWBG_BOT.Storage.Implementations
{
    public class InMemoryStorage : IDataStore
    {

        public InMemoryStorage()
        {
            Console.WriteLine("IMS Constructed!.");
        }

        private readonly Dictionary<string, object> _dictio = new Dictionary<string, object>();

        public void StoreQuiz(object obj, string key)
        {
            if (_dictio.ContainsKey(key)) return;
            _dictio.Add(key,obj);
        }

        public T RestoreQuiz<T>(string key)
        {
            if (!_dictio.ContainsKey(key)) throw new ArgumentException($"The provided "+key+" not found");
             
            return (T)_dictio[key];
        }

    }
}
