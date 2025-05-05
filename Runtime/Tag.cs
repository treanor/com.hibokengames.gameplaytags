using System;
using UnityEngine;

namespace HibokenGames.GameplayTags
{
    /// <summary>
    /// Represents a hierarchical gameplay tag.
    /// Tags follow a dot notation format (e.g., "Enemy.Melee.Aggressive").
    /// </summary>
    [Serializable]
    public class Tag : IEquatable<Tag>
    {
        [SerializeField, Tooltip("The full tag name in dot notation format (e.g., 'Enemy.Melee.Aggressive')")]
        private string name;

        /// <summary>
        /// Gets the full tag name (e.g., "Enemy.Melee.Aggressive").
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Creates a new gameplay tag.
        /// </summary>
        /// <param name="tagName">The full tag name in dot notation format (e.g., "Enemy.Melee.Aggressive")</param>
        public Tag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));
            }
            name = tagName.Trim();
        }

        /// <summary>
        /// Determines if this tag is a parent of or the same as the other tag.
        /// </summary>
        /// <param name="other">The tag to check against</param>
        /// <returns>True if this tag is a parent of or the same as the other tag</returns>
        public bool IsParentOf(Tag other)
        {
            if (other == null) return false;
            
            // Either tags are equal, or the other tag starts with this tag followed by a '.'
            return other.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) ||
                  (other.Name.StartsWith(Name + ".", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines if this tag is a child of the other tag.
        /// </summary>
        /// <param name="other">The tag to check against</param>
        /// <returns>True if this tag is a child of the other tag</returns>
        public bool IsChildOf(Tag other)
        {
            return other != null && other.IsParentOf(this) && !Equals(other);
        }

        public override string ToString() => Name;

        public override bool Equals(object obj) => Equals(obj as Tag);

        public bool Equals(Tag other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Name ?? string.Empty);
        }

        public static bool operator ==(Tag left, Tag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tag left, Tag right)
        {
            return !Equals(left, right);
        }
    }
}
