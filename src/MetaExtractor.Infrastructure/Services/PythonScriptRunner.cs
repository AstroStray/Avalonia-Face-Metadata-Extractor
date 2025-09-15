using System.Diagnostics;
using System.Threading.Tasks;
using MetaExtractor.Core.Services;

namespace MetaExtractor.Infrastructure.Services;

public class PythonScriptRunner : IPythonScriptRunner
{
    public async Task<string> RunScriptAsync(string scriptPath, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{scriptPath}\" {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();

        string result = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrEmpty(error))
        {
            throw new System.Exception($"Python script error: {error}");
        }

        return result;
    }
}
