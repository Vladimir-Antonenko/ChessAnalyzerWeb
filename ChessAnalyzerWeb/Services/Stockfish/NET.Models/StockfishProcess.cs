using System;
using System.Diagnostics;

namespace ChessAnalyzerApi.Services.Stockfish.NET.Models;

internal class StockfishProcess
{
    ProcessStartInfo _processStartInfo { get; init; } // Default process info for Stockfish process
    private Process Process { get; init; } // Stockfish process

    /// <summary>
    /// Stockfish process constructor
    /// </summary>
    /// <param name="path">Path to usable binary file from stockfish site</param>
    public StockfishProcess(string path)
    {
        //TODO: need add method which should be depended on os version
        _processStartInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        Process = new Process { StartInfo = _processStartInfo };
    }

    /// <summary>
    /// This method is writing in stdin of Stockfish process
    /// </summary>
    /// <param name="command"></param>
    public void WriteLine(string command)
    {
        if (Process.StandardInput == null)
        {
            throw new NullReferenceException();
        }
        Process.StandardInput.WriteLine(command);
        Process.StandardInput.Flush();
    }

    public string ReadLine() => Process.StandardOutput == null ?  // This method is allowing to read stdout of Stockfish process
        throw new NullReferenceException() : Process.StandardOutput.ReadLine()!;
    
    public void Start() => Process.Start(); // Start stockfish process
    public void Wait(int millisecond) => Process.WaitForExit(millisecond);

    ~StockfishProcess() => Process.Close(); // This method is allowing to close Stockfish process. When process is going to be destructed => we are going to close stockfish process
}
