using UnityEngine;

namespace HibokenGames.GameplayTags
{
    /// <summary>
    /// Component that attaches a TagContainer to a GameObject, providing convenient tag-related functionality.
    /// </summary>
    [AddComponentMenu("Hiboken Games/Tag Container")]
    public class TagContainerComponent : MonoBehaviour
    {
        [SerializeField, Tooltip("The collection of gameplay tags associated with this GameObject")]
        private TagContainer container = new TagContainer();
        
        /// <summary>
        /// Gets the tag container associated with this component.
        /// </summary>
        public TagContainer Container => container;
        
        /// <summary>
        /// Checks if the container has a tag with the exact name.
        /// </summary>
        /// <param name="tagName">The name of the tag to check</param>
        /// <returns>True if the container has the tag, false otherwise</returns>
        public bool HasTag(string tagName) => container.HasTag(tagName);
        
        /// <summary>
        /// Checks if the container has a tag that matches any part of the specified hierarchy.
        /// </summary>
        /// <param name="tagName">The tag to match against</param>
        /// <returns>True if there's a matching tag, false otherwise</returns>
        public bool HasTagInHierarchy(string tagName) => container.HasTagInHierarchy(tagName);
        
        /// <summary>
        /// Adds a tag to the container.
        /// </summary>
        /// <param name="tagName">The name of the tag to add</param>
        public void AddTag(string tagName) => container.AddTag(tagName);
        
        /// <summary>
        /// Adds multiple tags to the container.
        /// </summary>
        /// <param name="tagNames">The tags to add</param>
        public void AddTags(params string[] tagNames) => container.AddTags(tagNames);
        
        /// <summary>
        /// Removes a tag from the container.
        /// </summary>
        /// <param name="tagName">The name of the tag to remove</param>
        /// <returns>True if the tag was removed, false if it wasn't in the container</returns>
        public bool RemoveTag(string tagName) => container.RemoveTag(tagName);
        
        /// <summary>
        /// Checks if this container has any of the tags in the specified container.
        /// </summary>
        /// <param name="other">The other container to check against</param>
        /// <returns>True if there's at least one matching tag, false otherwise</returns>
        public bool MatchesAny(TagContainer other) => container.MatchesAny(other);
        
        /// <summary>
        /// Checks if this container has any of the tags in the specified component's container.
        /// </summary>
        /// <param name="other">The other component to check against</param>
        /// <returns>True if there's at least one matching tag, false otherwise</returns>
        public bool MatchesAny(TagContainerComponent other) => other != null && container.MatchesAny(other.Container);
        
        /// <summary>
        /// Checks if this container has all of the tags in the specified container.
        /// </summary>
        /// <param name="other">The other container to check against</param>
        /// <returns>True if all tags match, false otherwise</returns>
        public bool MatchesAll(TagContainer other) => container.MatchesAll(other);
        
        /// <summary>
        /// Checks if this container has all of the tags in the specified component's container.
        /// </summary>
        /// <param name="other">The other component to check against</param>
        /// <returns>True if all tags match, false otherwise</returns>
        public bool MatchesAll(TagContainerComponent other) => other != null && container.MatchesAll(other.Container);
        
        /// <summary>
        /// Clears all tags from the container.
        /// </summary>
        public void ClearTags() => container.ClearTags();
    }
}