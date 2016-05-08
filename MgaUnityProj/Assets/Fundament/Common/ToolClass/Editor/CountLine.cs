using System.IO;
using UnityEditor;
using UnityEngine;

public class CountLine
{
    [MenuItem("MGA/CMDs/Common/Count Line")]
    static void CountLineFunc()
    {
        int iFileNumber = 0;
        int iLineNumber = 0;
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Fundament/", "*.cs",
                                         SearchOption.AllDirectories);
        if (null != filePaths)
        {
            foreach (string filePath in filePaths)
            {
                ++iFileNumber;
                string[] lines = File.ReadAllLines(filePath);
                iLineNumber += lines.Length;
            }
        }

        filePaths = Directory.GetFiles(Application.dataPath + "/GameAssets/", "*.cs",
                                 SearchOption.AllDirectories);
        if (null != filePaths)
        {
            foreach (string filePath in filePaths)
            {
                ++iFileNumber;
                string[] lines = File.ReadAllLines(filePath);
                iLineNumber += lines.Length;
            }
        }

        filePaths = Directory.GetFiles(Application.dataPath + "/Games/", "*.cs",
                         SearchOption.AllDirectories);
        if (null != filePaths)
        {
            foreach (string filePath in filePaths)
            {
                ++iFileNumber;
                string[] lines = File.ReadAllLines(filePath);
                iLineNumber += lines.Length;
            }
        }

        CRuntimeLogger.Log(string.Format("cs: {0}Files,{1}Lines", iFileNumber, iLineNumber));
    }
}