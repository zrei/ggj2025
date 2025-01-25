using UnityEngine;

public delegate void IntEvent(int _);
public delegate void VoidEvent();
public delegate void FloatEvent(float _);
public delegate void BoolEvent(bool _);
public delegate void Vector3Event(Vector3 _);

public static class GlobalEvents 
{
    public static class Player
    {
        public static int PlayerMaxHealth = 100;
        public static float ReviveTime = 5f;
        public static float DirtyDotInterval = 2f;
        public static int DirtyDotDamage = 1;
        public static VoidEvent OnPlayerStartSliding;
        public static VoidEvent OnPlayerStopSliding;
        public static VoidEvent OnPlayerShoot;
        public static VoidEvent OnPlayerMove;
        public static VoidEvent OnPlayerDeath;
        public static BoolEvent OnPlayerMoveOntoTile;
    }
}