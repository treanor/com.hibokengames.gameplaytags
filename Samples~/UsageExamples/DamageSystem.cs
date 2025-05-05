using UnityEngine;
using HibokenGames.GameplayTags;

namespace HibokenGames.GameplayTags.Samples
{
    /// <summary>
    /// Example showing how to implement a damage system using gameplay tags.
    /// </summary>
    public class DamageSystem : MonoBehaviour
    {
        [SerializeField]
        private TagContainer damageTypeTags = new TagContainer();

        [Tooltip("Multiplier for damage against specific tags")]
        [SerializeField]
        private DamageMultiplier[] damageMultipliers;

        public float CalculateDamageMultiplier(TagContainerComponent targetTags)
        {
            if (targetTags == null) return 1.0f;

            float multiplier = 1.0f;
            
            // Check each damage multiplier
            foreach (var damageMultiplier in damageMultipliers)
            {
                if (targetTags.HasTagInHierarchy(damageMultiplier.targetTag))
                {
                    multiplier *= damageMultiplier.multiplier;
                }
            }
            
            return multiplier;
        }

        public void ApplyDamage(GameObject target, float baseDamage)
        {
            // Try to get the target's health component
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth == null) return;
            
            // Try to get the target's tag container
            TagContainerComponent targetTags = target.GetComponent<TagContainerComponent>();
            
            // Calculate damage with multipliers
            float multiplier = CalculateDamageMultiplier(targetTags);
            float finalDamage = baseDamage * multiplier;
            
            // Apply the damage
            targetHealth.TakeDamage(finalDamage, damageTypeTags);
            
            Debug.Log($"Applied {finalDamage} damage to {target.name} (Base: {baseDamage}, Multiplier: {multiplier})");
        }
    }

    /// <summary>
    /// Simple class to store damage multipliers against specific tags.
    /// </summary>
    [System.Serializable]
    public class DamageMultiplier
    {
        public string targetTag;
        public float multiplier = 1.0f;
    }
}