using UnityEngine;


public class Persistant : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
