using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DWave))]
public class DWaveEditor : MultiTextBoxEditor<DWave> { }
#endif