using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HoldableObject : MonoBehaviour, IInteractable
{
    public bool isHeld = false;
    [SerializeField] private float damage = 0.0f;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void Interact(PlayerController playerController)
    {
        PlayerMelee playerMelee = playerController.playerMelee;
        if(!playerMelee.isHoldingObject)
        {
            playerMelee.isHoldingObject = true;
            isHeld = true; 
            rb.isKinematic = true; 
            col.enabled = false; 
            
            transform.parent = playerMelee.ObjectHolder;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

        }
        //pick up attach to player hand
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHeld) return;
        if (collision.gameObject.CompareTag("Player")) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) return;
        IDamageable enemy = collision.gameObject.GetComponentInParent<IDamageable>();
        if (enemy != null && damage > 0)
        {
            enemy.Damage(damage);
        }
    }
}
