using System.Diagnostics;
using System.IO;

namespace local_translate_provider;

/// <summary>
/// 可选调试日志，写入 %TEMP%\local-translate-provider-debug.log。
/// 默认关闭，通过环境变量 LOCAL_TRANSLATE_PROVIDER_DEBUG_LOG=1 或 CLI --debug-log 启用。
/// </summary>
internal static class DebugLog
{
    private static readonly object Lock = new();
    private static string? _logPath;

    private static string LogPath => _logPath ??= Path.Combine(
        Path.GetTempPath(),
        "local-translate-provider-debug.log");

    /// <summary>
    /// 是否启用。由环境变量 LOCAL_TRANSLATE_PROVIDER_DEBUG_LOG 控制（1/true/yes 为启用）。
    /// </summary>
    public static bool IsEnabled { get; set; }

    public static void Write(string message)
    {
        if (!IsEnabled) return;
        try
        {
            var line = $"[{DateTime.Now:HH:mm:ss.fff}] [{Environment.ProcessId}] {message}";
            lock (Lock)
            {
                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
#if DEBUG
            Debug.WriteLine(line);
#endif
        }
        catch { }
    }
}
