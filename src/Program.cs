using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

class Program
{

    [STAThread]
    static void Main(string[] args)
    {

        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
        .AddUserSecrets<Program>()
        .Build();



        var rootCommand = new RootCommand("Token generator for local, uat, production environments.");
        var nameOption = new Option<string>(["--environment", "-e"], () => "production", "The environment where the IDP is.");
        var greetingOption = new Option<string>(["--secret", "-s"], () => "password", "The client secret to authenticate with.");

        rootCommand.AddOption(nameOption);
        rootCommand.AddOption(greetingOption);

        rootCommand.Handler = CommandHandler.Create<string, string>((environment, secret) =>
        {
            services.ConfigureServices(configuration, new[] { environment, secret });
            var serviceProvider = services.BuildServiceProvider();
            var tokenService = serviceProvider.GetService<ITokenService>();
            var accessToken = tokenService.GenerateToken().GetAwaiter().GetResult();


            Clipboard.SetText(accessToken.ToString());
            Console.WriteLine(accessToken);
            Console.WriteLine("Token written to clipboard");
        });

        rootCommand.Invoke(args);

    }
}

