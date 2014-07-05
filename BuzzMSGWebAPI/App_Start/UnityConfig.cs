using BuzzMSGData;
using BuzzMSGEntity.Models;
using BuzzMSGServices;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Unity.WebApi;

namespace BuzzMSGWebAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IRepository<BuzzUser>, Repository<BuzzUser>>();
            container.RegisterType<IDataContext, BuzzMsgdbContext>();
            container.RegisterType<IDataContextAsync, BuzzMsgdbContext>();
            container.RegisterType<IRepository<Buzz>, Repository<Buzz>>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>();
            container.RegisterType<IBuzzService, BuzzService>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}