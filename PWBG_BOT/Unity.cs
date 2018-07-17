using Unity;
using Unity.Resolution;
using PWBG_BOT.Storage;
using PWBG_BOT.Storage.Implementations;
namespace PWBG_BOT
{
    public static class Unity
    {
        private static UnityContainer _cont;

        public static UnityContainer Container {
            get
            {
                if(_cont == null)
                {
                    RegisterTypes();
                }
                return _cont;
            }
        }

        public static void RegisterTypes()
        {
            _cont = new UnityContainer();
            _cont.RegisterType<IDataStore, InMemoryStorage>();
        }

        public static T Resolve<T>()
        {
            return (T)Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }

    }
}
