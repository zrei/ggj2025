using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBeam : MonoBehaviour
{
    private void ShootGun()
    {
        // TODO
        GlobalEvents.Player.OnPlayerShoot?.Invoke();
    }
}
