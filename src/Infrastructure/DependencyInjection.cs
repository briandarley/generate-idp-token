using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using generate_idp_token.Pocos;

public static class DependencyInjection
{

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, string[] args)
    {

        services.AddHttpClient();
        ConfigurHttpClient(services, configuration, args[0], args[1]);
        services.AddSingleton<ITokenService, TokenService>();

    }

    private static void ConfigurHttpClient(IServiceCollection services, IConfiguration configuration, string env, string secret)
    {

        //TODO when setting up new APP
        //Initialize user secrets using the below command
        //dotnet user-secrets init
        //dotnet user-secrets set "LOCAL_IDP_CLIENT_ID" "local_idp_client_id"
        //dotnet user-secrets set "LOCAL_IDP_CLIENT_SECRET" "local_idp_client_secret"
        //dotnet user-secrets set "PRD_IDP_CLIENT_ID" "INTERNAL_API"
        //dotnet user-secrets set "PRD_IDP_CLIENT_SECRET" "password"
        //To list curent secrets use the below command
        //dotnet user-secrets list


        var idpConfigurations = configuration.GetSection("IdpConfigurations").Get<List<IdpConfiguration>>();
        if (idpConfigurations is null)
        {
            Console.WriteLine("IdpConfigurations is null");
            throw new ArgumentNullException("IdpConfigurations is null");
        }

        //Retrieve configuration from user secrets
        idpConfigurations.Single(c => c.Name == "LOCAL_IDP").ClientId = configuration["LOCAL_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("LOCAL_IDP:CLIENT_ID");
        idpConfigurations.Single(c => c.Name == "LOCAL_IDP").ClientSecret = configuration["LOCAL_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("LOCAL_IDP:CLIENT_SECRET");

        idpConfigurations.Single(c => c.Name == "UAT_IDP").ClientId = configuration["UAT_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("UAT_IDP:CLIENT_ID");
        idpConfigurations.Single(c => c.Name == "UAT_IDP").ClientSecret = configuration["UAT_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("UAT_IDP:CLIENT_ID");

        idpConfigurations.Single(c => c.Name == "PRD_IDP").ClientId = configuration["PRD_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("PRD_IDP:CLIENT_ID");
        idpConfigurations.Single(c => c.Name == "PRD_IDP").ClientSecret = configuration["PRD_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("PRD_IDP:CLIENT_ID");
        IdpConfiguration idpConfig = null;
        switch (env.ToUpper())
        {
            case "LOCAL":
                idpConfig = idpConfigurations.Single(c => c.Name == "LOCAL_IDP");
                idpConfig.ClientId = configuration["LOCAL_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("LOCAL_IDP:CLIENT_ID");
                idpConfig.ClientSecret = configuration["LOCAL_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("LOCAL_IDP:CLIENT_SECRET");



                break;
            case "UAT":

                idpConfig = idpConfigurations.Single(c => c.Name == "UAT_IDP");
                idpConfig.ClientId = configuration["UAT_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("UAT_IDP:CLIENT_ID");
                idpConfig.ClientSecret = configuration["UAT_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("UAT_IDP:CLIENT_SECRET");
                break;
            case "TST":
                idpConfig = idpConfigurations.Single(c => c.Name == "TST_IDP");
                idpConfig.ClientId = configuration["TST_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("TST_IDP:CLIENT_ID");
                idpConfig.ClientSecret = configuration["TST_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("TST_IDP:CLIENT_SECRET");


                break;
            case "PROD":
            case "PRODUCTION":
            case "PRD":
                idpConfig = idpConfigurations.Single(c => c.Name == "PRD_IDP");
                idpConfig.ClientId = configuration["PRD_IDP:CLIENT_ID"] ?? Environment.GetEnvironmentVariable("PRD_IDP:CLIENT_ID");
                idpConfig.ClientSecret = configuration["PRD_IDP:CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("PRD_IDP:CLIENT_SECRET");


                break;
            default:
                throw new ArgumentException("Invalid environment");
        }


        services.AddHttpClient("IDP", client =>
        {
            client.BaseAddress = new Uri(idpConfig.BaseAddress);
            client.Timeout = new TimeSpan(0, 1, 0);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });


        services.AddSingleton(idpConfig);


    }





}