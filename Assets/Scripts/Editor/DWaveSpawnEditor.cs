using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DWaveSpawn))]
public class DWaveSpawnEditor : MultiTextBoxEditor<DWaveSpawn> { }
#endif