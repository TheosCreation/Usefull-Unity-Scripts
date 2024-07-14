using System;

public interface IDamageable
{
    // Property to get and set the health of the object.
    float Health { get; set; }

    // Method to apply damage to the object.
    void Damage(float damageAmount);

    // Optional: Method to heal the object.
    void Heal(float healAmount);

    // Optional: Event to notify when the object's health reaches 0.
    event Action OnDeath;

    // Optional: Event to notify whenever the object takes damage.
    //event Action<int> OnTakeDamage;
}