using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;

public static class UpdateNotifier
{
    private const string LOCAL_VERSION_PATH = "Assets/MeshDecimater_Unity/VERSION";
    private const string REMOTE_VERSION_URL = "https://raw.githubusercontent.com/refiaa/MeshOptimizer_Unity/main/VERSION";

    private static string localVersion = "Unknown";
    private static string remoteVersion = "Unknown";
    private static bool isLatest = true;
    private static bool hasChecked = false;

    public static async void CheckForUpdates()
    {
        hasChecked = false;
        localVersion = GetLocalVersion();
        remoteVersion = await GetRemoteVersion();

        if (TryParseVersion(localVersion, out Version localVer) && TryParseVersion(remoteVersion, out Version remoteVer))
        {
            isLatest = localVer.CompareTo(remoteVer) >= 0;
        }
        else
        {
            isLatest = true;
        }

        // Debug.Log($"[UpdateNotifier] Local Version: {localVersion}");
        // Debug.Log($"[UpdateNotifier] Remote Version: {remoteVersion}");
        // Debug.Log($"[UpdateNotifier] Is Latest: {isLatest}");

        hasChecked = true;
    }

    private static string GetLocalVersion()
    {
        try
        {
            if (File.Exists(LOCAL_VERSION_PATH))
            {
                return File.ReadAllText(LOCAL_VERSION_PATH).Trim();
            }
            else
            {
                Debug.LogWarning($"[UpdateNotifier] Local VERSION file not found at path: {LOCAL_VERSION_PATH}");
                return "Unknown";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UpdateNotifier] Error reading local VERSION file: {ex.Message}");
            return "Unknown";
        }
    }

    private static async Task<string> GetRemoteVersion()
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                string version = await client.DownloadStringTaskAsync(REMOTE_VERSION_URL);
                return version.Trim();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UpdateNotifier] Error fetching remote VERSION file: {ex.Message}");
            return "Unknown";
        }
    }

    public static string GetUpdateStatus()
    {
        if (!hasChecked)
        {
            CheckForUpdates();
            return "Checking for updates...";
        }

        if (!isLatest && TryParseVersion(localVersion, out Version localVer) && TryParseVersion(remoteVersion, out Version remoteVer))
        {
            return $"Please update to the latest version: {localVer} -> {remoteVer}";
        }

        return string.Empty;
    }

    private static bool TryParseVersion(string versionStr, out Version version)
    {
        return Version.TryParse(versionStr, out version);
    }
}
