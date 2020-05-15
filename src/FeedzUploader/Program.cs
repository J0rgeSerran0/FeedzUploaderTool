using Feedz.Client;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FeedzUploader
{
    public class Program
    {
        private static string _apiKey = String.Empty;
        private static string _organization = String.Empty;
        private static string _path = String.Empty;
        private static string _repository = String.Empty;

        public static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{nameof(FeedzUploader)} - Tool to upload your NuGet packages to Feedz (https://feedz.io/)");
            Console.WriteLine();
            Console.ResetColor();

            var checkArgumentsResult = CheckArguments(args);

            if (!checkArgumentsResult.IsReady &&
                checkArgumentsResult.ShowHelp)
            { 
                WriteHelp();
                if (!String.IsNullOrWhiteSpace(checkArgumentsResult.Message))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(checkArgumentsResult.Message);
                    Console.WriteLine();
                    Console.ResetColor();
                }
            }
            else if (!checkArgumentsResult.IsReady)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(checkArgumentsResult.Message);
                Console.WriteLine();
                Console.ResetColor();
            }
            else
            {
                await UploadPackagesAsync();

                Console.WriteLine("Process completed!");
            }
        }

        private static CheckArgumentsResult CheckArguments(string[] args)
        {
            if (args.Count() != 4)
                return new CheckArgumentsResult(false, true, $"Number of arguments found: {args.Count()}");

            _organization = args[0];
            _repository = args[1];
            _apiKey = args[2];
            _path = args[3];

            if (!Directory.Exists(_path))
                return new CheckArgumentsResult(false, false, $"The path {_path} does not exist");

            return new CheckArgumentsResult(true);
        }

        private static void WriteHelp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Use this tool as:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{nameof(FeedzUploader)} <organization> <repository> <apiKey> <path>");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t<organization>: organization of your feedz repo");
            Console.WriteLine("\t<repository>: organization's repository of your feedz repo");
            Console.WriteLine("\t<apiKey>: API Key with read/write permissions to upload your NuGet packages into your repository");
            Console.WriteLine("\t<path>: path where your NuGet packages are ready to be uploaded (*.nupkg)");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{nameof(FeedzUploader)} acmeorg acmerepo T-aaaaaaaaaa1111111111bbbbbb2222222 C:\\temp\\");
            Console.WriteLine();
            Console.ResetColor();
        }

        private static async Task UploadPackagesAsync()
        {
            try
            {
                var client = FeedzClient.Create(_apiKey);
                RepositoryScope repositoryScope = client.ScopeToRepository(_organization, _repository);

                // Load *ALL* the New Packages
                var files = Directory.GetFiles(_path);
                var numberOfFiles = files.Count();
                var counterFiles = 1;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{numberOfFiles} (*.nupkg) file(s) found!");
                Console.WriteLine();

                foreach (var file in files)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"[{counterFiles}] Uploading {file}...");
                    try
                    {
                        await repositoryScope.PackageFeed.Upload(file);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{file} correctly uploaded!");
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        if (ex.Message == "Forbidden")
                            Console.WriteLine("Please, check the API Key to access and write into your own repo");
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                    counterFiles += 1;
                }

                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error ocurred!");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.ResetColor();
            }
        }
    }
}