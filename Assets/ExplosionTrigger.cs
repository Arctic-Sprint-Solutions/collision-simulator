using UnityEngine;
using RayFire;

// Testing explosion function during collision
public class ExplosionTrigger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        RayfireRigid rfRigid = GetComponent<RayfireRigid>();
        if (rfRigid != null)
        {
            rfRigid.Demolish();
        }
    }
}
