using UnityEngine;

public class KillDamageableOnTouch : MonoBehaviour
{
    private int damage = 100;
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage);
        }
    }
}
