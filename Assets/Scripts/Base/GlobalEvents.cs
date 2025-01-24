using UnityEngine;

public delegate void IntEvent(int _);
public delegate void VoidEvent();
public delegate void FloatEvent(float _);
public delegate void Vector3Event(Vector3 _);

public static class GlobalEvents 
{
    public static class Player
    {
        public static VoidEvent OnPlayerStartSliding;
        public static VoidEvent OnPlayerStopSliding;
    }
}