namespace LoanManagement.restApi.Configs.Services;

public static class ConfigAutofac
{
    public static ConfigureHostBuilder AddAutofac(
        this ConfigureHostBuilder builder,
        string connectionString)
    {
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.ConfigureContainer<ContainerBuilder>(b =>
            b.RegisterModule(new AutofacBusinessModule(connectionString))
        );
        return builder;
    }
}