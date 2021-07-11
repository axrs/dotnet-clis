using CommandDotNet;
using CommandDotNet.NameCasing;

namespace TimeTrackingHelper
{
    static class Program
    {
        static int Main(string[] args)
        {
            return new AppRunner<CommandLineInterface>()
                .UseDefaultMiddleware()
                .UseNameCasing(Case.KebabCase)
                .UseDefaultsFromEnvVar()
                .Run(args);
        }
    }
}