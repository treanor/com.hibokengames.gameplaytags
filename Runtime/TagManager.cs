using System;
using System.Collections.Generic;
using UnityEngine;

namespace HibokenGames.GameplayTags
{
    /// <summary>
    /// Singleton manager for registering and retrieving gameplay tags in the game.
    /// </summary>
    public class TagManager : MonoBehaviour
    {
        private static TagManager instance;

        /// <summary>
        /// Gets the singleton instance of the TagManager, creating it if necessary.
        /// </summary>
        public static TagManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("TagManager");
                    instance = go.AddComponent<TagManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private readonly Dictionary<string, Tag> registeredTags = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Registers a new tag with the system. If the tag already exists, the existing instance is returned.
        /// </summary>
        /// <param name="tagName">The name of the tag to register (e.g., "Enemy.Melee.Aggressive")</param>
        /// <returns>The registered tag instance</returns>
        public Tag RegisterTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));
            }

            string normalizedTagName = tagName.Trim();
            
            if (registeredTags.TryGetValue(normalizedTagName, out Tag existingTag))
            {
                return existingTag;
            }

            Tag newTag = new Tag(normalizedTagName);
            registeredTags.Add(normalizedTagName, newTag);
            return newTag;
        }

        /// <summary>
        /// Gets a registered tag by name.
        /// </summary>
        /// <param name="tagName">The name of the tag to retrieve</param>
        /// <returns>The tag if found, null otherwise</returns>
        public Tag GetTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return null;
            }

            registeredTags.TryGetValue(tagName.Trim(), out Tag tag);
            return tag;
        }

        /// <summary>
        /// Checks if a tag with the given name is registered.
        /// </summary>
        /// <param name="tagName">The name of the tag to check</param>
        /// <returns>True if the tag exists, false otherwise</returns>
        public bool HasTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return false;
            }

            return registeredTags.ContainsKey(tagName.Trim());
        }

        /// <summary>
        /// Gets all registered tags.
        /// </summary>
        /// <returns>A collection of all registered tags</returns>
        public IReadOnlyCollection<Tag> GetAllTags()
        {
            return registeredTags.Values;
        }

        /// <summary>
        /// Finds all tags that are children of the specified parent tag.
        /// </summary>
        /// <param name="parentTagName">The parent tag name to match against</param>
        /// <returns>A collection of matching child tags</returns>
        public ICollection<Tag> FindChildTags(string parentTagName)
        {
            if (string.IsNullOrWhiteSpace(parentTagName))
            {
                return Array.Empty<Tag>();
            }

            List<Tag> matchingTags = new List<Tag>();
            Tag parentTag = GetTag(parentTagName) ?? new Tag(parentTagName.Trim());

            foreach (Tag tag in registeredTags.Values)
            {
                if (tag.IsChildOf(parentTag))
                {
                    matchingTags.Add(tag);
                }
            }

            return matchingTags;
        }

        /// <summary>
        /// Clears all registered tags. Primarily for testing purposes.
        /// </summary>
        public void ClearAllTags()
        {
            registeredTags.Clear();
        }
    }
}