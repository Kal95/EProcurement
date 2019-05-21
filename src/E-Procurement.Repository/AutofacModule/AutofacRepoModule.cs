using Autofac;
using E_Procurement.Data;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.AutofacModule
{
    public class AutofacRepoModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EProcurementContext>().InstancePerLifetimeScope();

            // register dependency convention
            builder.RegisterAssemblyTypes(typeof(IDependencyRegister).Assembly)
                .AssignableTo<IDependencyRegister>()
                .As<IDependencyRegister>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            base.Load(builder);
           
        }
    }

    
}
