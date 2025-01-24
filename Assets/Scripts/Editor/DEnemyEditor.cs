using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DEnemy))]
public class DEnemyEditor : MultiTextBoxEditor<DEnemy> { }
#endif