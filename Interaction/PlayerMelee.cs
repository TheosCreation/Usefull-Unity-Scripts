using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerController playerController;
    private Animator animator;
    private Timer meleeTimer;
    private Timer throwTimer;
    [SerializeField] private bool canMelee = true;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float meleeDelay = 0.5f;
    [SerializeField] private float meleeRange = 0.5f;
    [SerializeField] private float meleeDamage = 10f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float enableObjectDelay = 0.05f;
    public Transform ObjectHolder;
    public bool isHoldingObject = false;
    [SerializeField] private GameObject bloodHitParticles;

    private HoldableObject objectToDrop;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.playerInputActions.Combat.Melee.started += _cx => Melee();

        playerController = GetComponent<PlayerController>();

        animator = GetComponent<Animator>();

        meleeTimer = this.AddComponent<Timer>();
        throwTimer = this.AddComponent<Timer>();
    }

    private void Melee()
    {
        if(canMelee)
        {

            if(isHoldingObject)
            {
                DropObject();
                return;
            }

            canMelee = false;

            animator.SetTrigger("Melee");

            RaycastHit hit;
            if (Physics.Raycast(playerController.playerCamera.transform.position, playerController.playerCamera.transform.forward, out hit, meleeRange, hitMask))
            {
                IDamageable damagable = hit.transform.GetComponentInParent<IDamageable>();
                if (damagable != null)
                {
                    damagable.Damage(meleeDamage);

                    Quaternion rotation = Quaternion.LookRotation(-transform.forward);

                    // Instantiate the hitParticles with the calculated rotation
                    GameObject hitparts = Instantiate(bloodHitParticles, hit.point, Quaternion.identity);

                    Destroy(hitparts, 0.5f);
                }

                IInteractable interactable = hit.transform.GetComponent<IInteractable>();
                if(interactable != null)
                {
                    interactable.Interact(playerController);
                }
            }

            meleeTimer.SetTimer(meleeDelay, ResetMelee);
        }
    }

    private void DropObject()
    {
        objectToDrop = ObjectHolder.GetComponentInChildren<HoldableObject>();
        if (objectToDrop)
        {

            objectToDrop.isHeld = false;
            objectToDrop.rb.isKinematic = false;
            objectToDrop.rb.AddForce(playerController.playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
            throwTimer.SetTimer(enableObjectDelay, EnableObject);
            ObjectHolder.DetachChildren();
        }

        isHoldingObject = false;
    }

    private void EnableObject()
    {
        objectToDrop.col.enabled = true;
        objectToDrop = null;
    }

    private void ResetMelee()
    {
        canMelee = true;
    }
}