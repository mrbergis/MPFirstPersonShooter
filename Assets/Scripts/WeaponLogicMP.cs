using Mirror;
using UnityEngine;

public class WeaponLogicMP : NetworkBehaviour
{
    WeaponLogic _weaponLogic;

    [SerializeField]
    Camera playerCamera;
    
    void Start()
    {
        _weaponLogic = GetComponentInChildren<WeaponLogic>();
    }
    
    [Command]
    public void CmdShoot()
    {
        if(!isServer || !_weaponLogic)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            Debug.Log("Hit object: " + rayHit.collider.name);
            Debug.Log("Hit Pos: " + rayHit.point);

            bool hitPlayer = rayHit.collider.tag == "Player";
            if (hitPlayer)
            {
                PlayerLogic playerLogic = rayHit.collider.GetComponent<PlayerLogic>();
                if(playerLogic)
                {
                    playerLogic.TakeDamage(30);
                }
            }

            RpcShootEffect(rayHit.point, 
                Quaternion.FromToRotation(Vector3.up, rayHit.normal) * Quaternion.Euler(-90, 0, 0), !hitPlayer);
        }
    }

    [ClientRpc]
    void RpcShootEffect(Vector3 impactPosition, Quaternion impactRotation, bool spawnImpactObj)
    {
        if(_weaponLogic)
        {
            _weaponLogic.ShootEffect(impactPosition, impactRotation, spawnImpactObj);
        }
    }
}
