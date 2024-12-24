using Module = Autofac.Module;

namespace LoanManagement.restApi.Configs.Services;

public class AutofacBusinessModule(string connectionString) : Module
{
    private const string ConnectionStringKey = "connectionString";


    protected override void Load(ContainerBuilder container)
    {
        var applicationAssembly = typeof(ApplicationAssembly).Assembly;
        var serviceAssembly = typeof(ServiceAssembly).Assembly;
        var persistenceAssembly = typeof(PersistenceAssembly).Assembly;

        container.RegisterAssemblyTypes(
                applicationAssembly,
                serviceAssembly)
            .AssignableTo<Service>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        container.RegisterAssemblyTypes(
                persistenceAssembly,
                serviceAssembly)
            .AssignableTo<Repository>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        container.RegisterAssemblyTypes(
                persistenceAssembly,
                serviceAssembly)
            .AssignableTo<IScope>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        container.RegisterType<EfDataContext>()
            .WithParameter(ConnectionStringKey, connectionString)
            .AsSelf()
            .InstancePerLifetimeScope();


        base.Load(container);
    }
}