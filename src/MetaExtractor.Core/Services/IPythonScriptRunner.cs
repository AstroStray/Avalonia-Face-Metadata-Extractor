using System.Threading.Tasks;

namespace MetaExtractor.Core.Services;

public interface IPythonScriptRunner
{
    /// <summary>
    /// Runs a Python script and returns its standard output.
    /// </summary>
    /// <param name="scriptPath">The path to the Python script.</param>
    /// <param name="arguments">The arguments to pass to the script.</param>
    /// <returns>A task representing the asynchronous operation. The result is the standard output of the script.</returns>
    Task<string> RunScriptAsync(string scriptPath, string arguments);
}
