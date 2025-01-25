using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


[CreateAssetMenu(fileName = "DEnemy", menuName = "Data/DEnemy", order = 3)]
public class DEnemy : ScriptableObject, IDataImport
{
    private static DEnemy s_loadedData;
    private static Dictionary<int, EnemyData> s_cachedDataDict;

    [field: SerializeField]
    public List<EnemyData> Data { get; private set; }

    public static DEnemy GetAllData()
    {
        if (s_loadedData == null)
        {
            // Load and cache results
            s_loadedData = Resources.Load<DEnemy>("data/denemy");

            // Calculate and cache some results
            s_cachedDataDict = new();
            foreach (var enemyData in s_loadedData.Data)
            {
#if UNITY_EDITOR
                if (s_cachedDataDict.ContainsKey(enemyData.Id))
                {
                    Debug.LogError($"Duplicate Id {enemyData.Id}");
                }
#endif
                s_cachedDataDict[enemyData.Id] = enemyData;
            }
        }

        return s_loadedData;
    }

    public static EnemyData? GetDataById(int id)
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

            // New item
            var enemyData = new EnemyData
            {
                Id = CommonUtil.ConvertToInt32(paramList[1]),
                Attack = CommonUtil.ConvertToInt32(paramList[2]),
                Hp = CommonUtil.ConvertToInt32(paramList[3]),
                AttackSpeed = CommonUtil.ConvertToSingle(paramList[4]),
                MovementSpeed = CommonUtil.ConvertToSingle(paramList [5]),
                ProjectileSpeed = CommonUtil.ConvertToSingle(paramList[6]),
                ProjectileTarget = Enum.TryParse(paramList[7], out ProjectileTarget projectileType) ? projectileType : ProjectileTarget.None,
                ProjectileLifetime = CommonUtil.ConvertToSingle(paramList[8]),
                NumberOfProjectiles = CommonUtil.ConvertToInt32(paramList[9]),
            };
            s_loadedData.Data.Add(enemyData);
        }

        CommonUtil.SaveScriptableObject(s_loadedData);
    }
#endif
}

[Serializable]
public struct EnemyData
{
    [field: SerializeField]
    public int Id { get; set; }

    [field: SerializeField]
    public int Attack { get; set; }

    [field: SerializeField]
    public int Hp {  get; set; }

    [field: SerializeField]
    public float AttackSpeed { get; set; }

    [field: SerializeField]
    public float MovementSpeed { get; set; }

    [field: SerializeField]
    public float ProjectileSpeed {  get; set; }

    [field: SerializeField]
    public ProjectileTarget ProjectileTarget { get; set; }

    [field: SerializeField]
    public float ProjectileLifetime {  get; set; }

    [field: SerializeField]
    public int NumberOfProjectiles { get; set; }
}

public enum ProjectileTarget
{
    Player,
    Enemy,
    DirtyTile,
    NeutralTile,
    CleanTile,
    NeutralOrCleanTile,
    None
}