using UnityEngine;
using HibokenGames.GameplayTags;
using System;

namespace HibokenGames.GameplayTags.Samples
{
    /// <summary>
    /// Example showing how to implement a health system using gameplay tags.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] 
        private float maxHealth = 100f;
        
        [SerializeField]
        private float currentHealth;
        
        [Tooltip("Tags that define resistances to specific damage types")]
        [SerializeField]
        private DamageResistance[] resistances;
        
        [Tooltip("Tags that define what this entity is vulnerable to")]
        [SerializeField]
        private TagContainer vulnerabilityTags = new TagContainer();
        
        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }
        
        /// <summary>
        /// Apply damage to this health component.
        /// </summary>
        /// <param name="damage">The amount of damage to apply</param>
        /// <param name="damageTypeTags">Tags describing the type of damage</param>
        public void TakeDamage(float damage, TagContainer damageTypeTags)
        {
            // Skip if already dead
            if (currentHealth <= 0)
                return;
                
            // Apply resistances based on damage tags
            float modifiedDamage = ApplyResistances(damage, damageTypeTags);
            
            // Apply vulnerability multiplier
            if (damageTypeTags != null && vulnerabilityTags.MatchesAny(damageTypeTags))
            {
                // 50% more damage when vulnerable to this damage type
                modifiedDamage *= 1.5f;
                Debug.Log($"{gameObject.name} is vulnerable to this damage type! Damage increased by 50%");
            }
            
            // Apply damage
            float previousHealth = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - modifiedDamage);
            
            // Notify of health change
            OnHealthChanged?.Invoke(previousHealth, currentHealth);
            
            Debug.Log($"{gameObject.name} took {modifiedDamage} damage, health: {currentHealth}/{maxHealth}");
            
            // Check for death
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Apply resistances to incoming damage.
        /// </summary>
        private float ApplyResistances(float damage, TagContainer damageTypeTags)
        {
            if (damageTypeTags == null || resistances.Length == 0)
                return damage;
                
            float resistanceMultiplier = 1.0f;
            
            foreach (var resistance in resistances)
            {
                // If the damage has a tag that matches this resistance
                if (damageTypeTags.HasTagInHierarchy(resistance.damageTypeTag))
                {
                    // Apply the resistance multiplier (less than 1 for damage reduction)
                    resistanceMultiplier *= resistance.resistanceMultiplier;
                    Debug.Log($"{gameObject.name} has resistance to {resistance.damageTypeTag}, multiplier: {resistance.resistanceMultiplier}");
                }
            }
            
            return damage * resistanceMultiplier;
        }
        
        /// <summary>
        /// Heal the character by the specified amount.
        /// </summary>
        public void Heal(float amount)
        {
            if (currentHealth <= 0)
                return; // Can't heal the dead
            
            float previousHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            
            OnHealthChanged?.Invoke(previousHealth, currentHealth);
            
            Debug.Log($"{gameObject.name} healed for {amount}, health: {currentHealth}/{maxHealth}");
        }
        
        private void Die()
        {
            Debug.Log($"{gameObject.name} has died!");
            OnDeath?.Invoke();
        }
    }
    
    /// <summary>
    /// Defines a resistance to a specific damage type.
    /// </summary>
    [Serializable]
    public class DamageResistance
    {
        public string damageTypeTag;
        
        [Tooltip("Values less than 1 reduce damage, greater than 1 increase damage")]
        [Range(0, 2)]
        public float resistanceMultiplier = 0.5f;
    }
}