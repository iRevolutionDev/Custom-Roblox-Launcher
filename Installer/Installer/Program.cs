using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

var UserAgent = new ProductInfoHeaderValue("(+Mozilla 5.0)");

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.UserAgent.Add(UserAgent);

// Fetches the roblox directory
Console.WriteLine("Fetching roblox directory...");
HttpResponseMessage GetRobloxVersion = await client.GetAsync("http://setup.roblox.com/version");
string RobloxVersion = await GetRobloxVersion.Content.ReadAsStringAsync();
string RobloxDir = $@"C:\Program Files (x86)\Roblox\Versions\{RobloxVersion}";
string SecondaryDir = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Roblox\Versions\{RobloxVersion}";

if (!Directory.Exists(RobloxDir))
{
    if (Directory.Exists(SecondaryDir)) RobloxDir = SecondaryDir;
    else
    {
        Console.WriteLine("Roblox is not installed!");
        Console.WriteLine("Do you want to install? Yes/No");
        
        string? response = Console.ReadLine();
        if (!string.IsNullOrEmpty(response) && response.ToLower() == "yes")
        {
            Process.Start(new ProcessStartInfo($"https://setup.rbxcdn.com/{RobloxVersion}-Roblox.exe") { UseShellExecute = true });
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

    }
}

Console.WriteLine("Roblox directory: " + RobloxDir);

// Fetches the latest launcher version
Console.WriteLine("Fetching latest launcher version...");
HttpResponseMessage GetLatestLauncherVersion = await client.GetAsync("https://api.github.com/repos/iRevolutionDev/Custom-Roblox-Launcher/releases/latest");
dynamic? JsonLatestLauncherVersion = JsonConvert.DeserializeObject(await GetLatestLauncherVersion.Content.ReadAsStringAsync());
string? DownloadUrl = JsonLatestLauncherVersion?.assets[0].browser_download_url;

Console.WriteLine("Latest launcher version: " + JsonLatestLauncherVersion?.name);

// Downloads the latest launcher
Console.WriteLine($"Downloading latest launcher version... ({DownloadUrl})");
HttpResponseMessage DownloadLatestLauncherVersion = await client.GetAsync(DownloadUrl);

if (!DownloadLatestLauncherVersion.IsSuccessStatusCode)
{
    Console.WriteLine("Failed to download latest launcher version!");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    Environment.Exit(0);
}

// Deletes the old launcher
Console.WriteLine("Deleting old launcher...");
File.Delete($@"{RobloxDir}\RobloxLauncherBeta.exe");

// Writes the file
Console.WriteLine("Writing new launcher...");
File.WriteAllBytes($@"{RobloxDir}\RobloxPlayerLauncher.exe", await DownloadLatestLauncherVersion.Content.ReadAsByteArrayAsync());

Console.WriteLine("Done!");
Console.ReadKey();