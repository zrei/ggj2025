using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


[CreateAssetMenu(fileName = "DWaveSpawn", menuName = "Data/DWaveSpawn", order = 3)]
public class DWaveSpawn : ScriptableObject, IDataImport
{
    private static DWaveSpawn s_loadedData;
    private static Dictionary<int, List<WaveSpawnData>> s_cachedDataDict;

    [field: SerializeField]
    public List<WaveSpawnData> Data { get; private set; }

    public static DWaveSpawn GetAllData()
    {
        if (s_loadedData == null)
        {
            // Load and cache results
            s_loadedData = Resources.Load<DWaveSpawn>("data/dwavespawn");

            // Calculate and cache some results
            s_cachedDataDict = new();
            foreach (var waveSpawnData in s_loadedData.Data)
            {
                if (!s_cachedDataDict.ContainsKey(waveSpawnData.Wave))
                {
                    s_cachedDataDict[waveSpawnData.Wave] = new List<WaveSpawnData>();
                }

                s_cachedDataDict[waveSpawnData.Wave].Add(waveSpawnData);
            }
        }

        return s_loadedData;
    }

    public static List<WaveSpawnData> GetDataById(int id)
    {
        if (s_cachedDataDict == null)
        {
            GetAllData();
        }

        return s_cachedDataDict.TryGetValue(id, out var result) ? result : null;
    }

#if UNITY_EDITOR
    public static void ImportData(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        s_loadedData = GetAllData();
        if (s_loadedData == null)
        {
            return;
        }

        if (s_loadedData.Data == null)
        {
            s_loadedData.Data = new();
        }
        else
        {
            s_loadedData.Data.Clear();
        }

        // special handling for shape parameter and percentage
        var pattern = @"[{}""]";
        text = text.Replace("\r\n", "\n");      // handle window line break
        text = text.Replace(",\n", ",");
        text = Regex.Replace(text, pattern, "");

        // Split data into lines
        var lines = text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            // Comment and Header
            if (lines[i][0].Equals('#') || lines[i][0].Equals('$'))
            {
                continue;
            }

            // Empty line
            var trimLine = lines[i].Trim();
            var testList = trimLine.Split('\t');
            if (testList.Length == 1 && string.IsNullOrEmpty(testList[0]))
            {
                continue;
            }

            // Split
            var paramList = lines[i].Split('\t');
            for (var j = 0; j < paramList.Length; j++)
            {
                paramList[j] = paramList[j].Trim();
            }

            var spawnPointSplit = paramList[3].Split(',');

            // New item
            var waveSpawnData = new WaveSpawnData
            {
                Wave = CommonUtil.ConvertToInt32(paramList[1]),
                EnemyId = CommonUtil.ConvertToInt32(paramList[2]),
                RowSpawnPoint = CommonUtil.ConvertToInt32(spawnPointSplit[0]),
                ColSpawnPoint = CommonUtil.ConvertToInt32(spawnPointSplit[1]),
                Delay = CommonUtil.ConvertToSingle(paramList[4]),
            };
            s_loadedData.Data.Add(waveSpawnData);
        }

        CommonUtil.SaveScriptableObject(s_loadedData);
    }
#endif
}

[Serializable]
public struct WaveSpawnData
{
    [field: SerializeField]
    public int Wave { get; set; }

    [field: SerializeField]
    public int EnemyId { get; set; }

    [field: SerializeField]
    public int RowSpawnPoint { get; set; }

    [field: SerializeField]
    public int ColSpawnPoint { get; set; }

    [field: SerializeField]
    public float Delay { get; set; }
}
