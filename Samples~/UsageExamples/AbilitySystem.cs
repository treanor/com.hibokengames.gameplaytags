using UnityEngine;
using HibokenGames.GameplayTags;
using System.Collections.Generic;

namespace HibokenGames.GameplayTags.Samples
{
    /// <summary>
    /// Example showing how to implement an ability system using gameplay tags.
    /// </summary>
    public class AbilitySystem : MonoBehaviour
    {
        [SerializeField]
        private List<Ability> abilities = new List<Ability>();
        
        [SerializeField]
        private TagContainer ownerTags = new TagContainer();
        
        private void Start()
        {
            // Register initial tags
            foreach (var ability in abilities)
            {
                ability.Initialize(this);
            }
        }
        
        public bool CanActivateAbility(Ability ability)
        {
            // Check cooldown
            if (ability.IsOnCooldown)
                return false;
                
            // Check if owner has required tags
            if (!ownerTags.MatchesAll(ability.OwnerRequiredTags))
                return false;
                
            // Check if owner has any blocked tags
            if (ownerTags.MatchesAny(ability.OwnerBlockedTags))
                return false;
                
            return true;
        }
        
        public void ActivateAbility(Ability ability)
        {
            if (!CanActivateAbility(ability))
                return;
                
            // Apply tags during activation
            foreach (var tag in ability.TagsToAddDuringActivation)
            {
                ownerTags.AddTag(tag);
            }
            
            // Perform ability effects
            ability.Activate();
            
            // Remove tags after activation
            foreach (var tag in ability.TagsToAddDuringActivation)
            {
                ownerTags.RemoveTag(tag);
            }
        }
        
        public TagContainer GetOwnerTags()
        {
            return ownerTags;
        }
    }
    
    /// <summary>
    /// Base class for abilities that can be activated.
    /// </summary>
    [System.Serializable]
    public class Ability
    {
        [SerializeField]
        private string abilityName;
        
        [SerializeField]
        private float cooldownDuration = 1.0f;
        
        [SerializeField]
        private TagContainer ownerRequiredTags = new TagContainer();
        
        [SerializeField]
        private TagContainer ownerBlockedTags = new TagContainer();
        
        [SerializeField]
        private List<string> tagsToAddDuringActivation = new List<string>();
        
        private float cooldownEndTime;
        private AbilitySystem abilitySystem;
        
        public bool IsOnCooldown => Time.time < cooldownEndTime;
        public TagContainer OwnerRequiredTags => ownerRequiredTags;
        public TagContainer OwnerBlockedTags => ownerBlockedTags;
        public List<string> TagsToAddDuringActivation => tagsToAddDuringActivation;
        
        public void Initialize(AbilitySystem system)
        {
            abilitySystem = system;
        }
        
        public virtual void Activate()
        {
            Debug.Log($"Activated ability: {abilityName}");
            cooldownEndTime = Time.time + cooldownDuration;
        }
    }
    
    /// <summary>
    /// Example damage ability that applies damage to a target.
    /// </summary>
    [System.Serializable]
    public class DamageAbility : Ability
    {
        [SerializeField]
        private float damageAmount = 10f;
        
        [SerializeField]
        private TagContainer damageTypeTags = new TagContainer();
        
        [SerializeField]
        private float range = 5f;
        
        public override void Activate()
        {
            base.Activate();
            
            // Find targets in range
            Collider[] colliders = Physics.OverlapSphere(
                transform.position, 
                range
            );
            
            foreach (var collider in colliders)
            {
                // Skip self
                if (collider.gameObject == gameObject)
                    continue;
                    
                // Try to get health component
                Health targetHealth = collider.GetComponent<Health>();
                if (targetHealth != null)
                {
                    // Apply damage
                    targetHealth.TakeDamage(damageAmount, damageTypeTags);
                }
            }
        }
    }
}