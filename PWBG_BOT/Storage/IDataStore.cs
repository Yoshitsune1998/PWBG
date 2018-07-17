
namespace PWBG_BOT.Storage
{
    public interface IDataStore
    {
        void StoreQuiz(object obj, string key);

        T RestoreQuiz<T>(string key);
    }
}
