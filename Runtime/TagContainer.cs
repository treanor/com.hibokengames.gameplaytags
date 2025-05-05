using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HibokenGames.GameplayTags
{
    /// <summary>
    /// Container for multiple gameplay tags, providing matching and query functionality.
    /// </summary>
    [Serializable]
    public class TagContainer
    {
        [SerializeField, Tooltip("The list of tags in this container")]
        private List<string> tagNames = new List<string>();

        private readonly List<Tag> tags = new List<Tag>();
        private bool isDirty = true;

        /// <summary>
        /// Gets all tags in this container.
        /// </summary>
        public IReadOnlyList<Tag> Tags
        {
            get
            {
                SyncTagsIfNeeded();
                return tags;
            }
        }

        /// <summary>
        /// Creates a new empty tag container.
        /// </summary>
        public TagContainer() { }

        /// <summary>
        /// Creates a new tag container with the specified tags.
        /// </summary>
        /// <param name="initialTags">The initial tags to add to the container</param>
        public TagContainer(IEnumerable<string> initialTags)
        {
            if (initialTags != null)
            {
                tagNames.AddRange(initialTags.Where(t => !string.IsNullOrWhiteSpace(t)));
                isDirty = true;
            }
        }

        /// <summary>
        /// Adds a tag to the container.
        /// </summary>
        /// <param name="tagName">The name of the tag to add</param>
        public void AddTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return;

            string normalizedTag = tagName.Trim();
            if (!tagNames.Contains(normalizedTag, StringComparer.OrdinalIgnoreCase))
            {
                tagNames.Add(normalizedTag);
                isDirty = true;
            }
        }

        /// <summary>
        /// Adds multiple tags to the container.
        /// </summary>
        /// <param name="newTagNames">The collection of tag names to add</param>
        public void AddTags(IEnumerable<string> newTagNames)
        {
            if (newTagNames == null) return;

            bool changed = false;
            foreach (string tagName in newTagNames)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;

                string normalizedTag = tagName.Trim();
                if (!tagNames.Contains(normalizedTag, StringComparer.OrdinalIgnoreCase))
                {
                    tagNames.Add(normalizedTag);
                    changed = true;
                }
            }

            if (changed)
            {
                isDirty = true;
            }
        }

        /// <summary>
        /// Removes a tag from the container.
        /// </summary>
        /// <param name="tagName">The name of the tag to remove</param>
        /// <returns>True if the tag was removed, false if it wasn't in the container</returns>
        public bool RemoveTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return false;

            string normalizedTag = tagName.Trim();
            bool removed = tagNames.RemoveAll(t => 
                string.Equals(t, normalizedTag, StringComparison.OrdinalIgnoreCase)) > 0;
            
            if (removed)
            {
                isDirty = true;
            }
            
            return removed;
        }

        /// <summary>
        /// Checks if the container has a tag with the exact name.
        /// </summary>
        /// <param name="tagName">The name of the tag to check</param>
        /// <returns>True if the container has the tag, false otherwise</returns>
        public bool HasTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return false;
            return tagNames.Contains(tagName.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the container has a tag that matches any part of the specified hierarchy.
        /// </summary>
        /// <param name="tagName">The tag to match against</param>
        /// <returns>True if there's a matching tag, false otherwise</returns>
        public bool HasTagInHierarchy(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return false;

            SyncTagsIfNeeded();
            Tag searchTag = new Tag(tagName);

            foreach (Tag tag in tags)
            {
                if (tag.IsParentOf(searchTag) || searchTag.IsParentOf(tag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the container has any of the tags in the specified container.
        /// </summary>
        /// <param name="other">The other container to check against</param>
        /// <returns>True if there's at least one matching tag, false otherwise</returns>
        public bool MatchesAny(TagContainer other)
        {
            if (other == null) return false;
            
            // Fast path for direct string matching
            foreach (string tagName in tagNames)
            {
                if (other.HasTag(tagName))
                {
                    return true;
                }
            }

            // Hierarchy matching
            SyncTagsIfNeeded();
            other.SyncTagsIfNeeded();

            foreach (Tag otherTag in other.Tags)
            {
                foreach (Tag tag in tags)
                {
                    if (tag.IsParentOf(otherTag) || otherTag.IsParentOf(tag))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the container has all of the tags in the specified container.
        /// </summary>
        /// <param name="other">The other container to check against</param>
        /// <returns>True if all tags match, false otherwise</returns>
        public bool MatchesAll(TagContainer other)
        {
            if (other == null || other.Tags.Count == 0) return true;
            if (tags.Count == 0) return false;

            SyncTagsIfNeeded();
            other.SyncTagsIfNeeded();

            foreach (Tag otherTag in other.Tags)
            {
                bool hasMatch = false;
                foreach (Tag tag in tags)
                {
                    if (tag.Equals(otherTag) || tag.IsParentOf(otherTag))
                    {
                        hasMatch = true;
                        break;
                    }
                }

                if (!hasMatch)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears all tags from the container.
        /// </summary>
        public void ClearTags()
        {
            if (tagNames.Count > 0)
            {
                tagNames.Clear();
                tags.Clear();
                isDirty = false;
            }
        }

        private void SyncTagsIfNeeded()
        {
            if (!isDirty) return;

            tags.Clear();
            foreach (string tagName in tagNames)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;
                
                Tag tag = TagManager.Instance.RegisterTag(tagName);
                tags.Add(tag);
            }

            isDirty = false;
        }
    }
}