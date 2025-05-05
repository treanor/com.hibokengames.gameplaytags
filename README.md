# Gameplay Tags for Unity

A lightweight, efficient, and modular gameplay tag system for Unity, inspired by Unreal Engine's Gameplay Tags. This package provides a robust way to categorize and query game elements using hierarchical tags.

## Installation

### Via Unity Package Manager (UPM)

1. Open the Package Manager window in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter the repository URL: `https://github.com/hibokengames/gameplaytags.git`
5. Click "Add"

### Manual Installation

1. Download or clone this repository
2. Copy the contents into the `Packages` folder in your Unity project

## Overview

Gameplay Tags provide a way to categorize and mark objects with hierarchical string identifiers. For example, an enemy might have tags like `Enemy.Melee.Aggressive` or `Enemy.Ranged.Defensive`.

The hierarchical nature (using dots as separators) allows for powerful matching logic, where you can check if one tag is a parent or child of another.

## Core Components

### Tag

A single gameplay tag representing a hierarchical identifier, such as `Enemy.Melee.Aggressive`.

```csharp
// Create a new tag
Tag enemyTag = new Tag("Enemy.Melee.Aggressive");

// Check if a tag is a parent of another
bool isParent = enemyTag.IsParentOf(new Tag("Enemy.Melee.Aggressive.Boss"));

// Check if a tag is a child of another
bool isChild = enemyTag.IsChildOf(new Tag("Enemy"));
```

### TagManager

A singleton manager that handles registration and retrieval of all gameplay tags in your game.

```csharp
// Get the tag manager
TagManager tagManager = TagManager.Instance;

// Register a tag
Tag enemyTag = tagManager.RegisterTag("Enemy.Melee.Aggressive");

// Check if a tag exists
bool hasTag = tagManager.HasTag("Enemy.Melee.Aggressive");

// Get a registered tag
Tag retrievedTag = tagManager.GetTag("Enemy.Melee.Aggressive");

// Find all child tags of a parent tag
ICollection<Tag> childTags = tagManager.FindChildTags("Enemy");
```

### TagContainer

A container that holds multiple tags and provides matching functionality.

```csharp
// Create a tag container
TagContainer container = new TagContainer();

// Add tags to the container
container.AddTag("Enemy.Melee");
container.AddTag("Item.Weapon.Sword");

// Add multiple tags at once
container.AddTags(new[] { "Player.Warrior", "Ability.Damage.Physical" });

// Check if the container has a specific tag
bool hasTag = container.HasTag("Enemy.Melee");

// Check if the container has a tag in a hierarchy
bool hasInHierarchy = container.HasTagInHierarchy("Enemy");

// Compare with another tag container
TagContainer otherContainer = new TagContainer(new[] { "Enemy.Ranged", "Player.Warrior" });
bool matchesAny = container.MatchesAny(otherContainer);
bool matchesAll = container.MatchesAll(otherContainer);
```

## Editor Integration

The package includes editor tools to make working with gameplay tags easier:

1. **Tag Manager Window**: An editor window for viewing and managing tags. Access it via Window > Hiboken Games > Gameplay Tags.
2. **Custom Property Drawer**: A user-friendly inspector for TagContainer fields in your components.

## Example Usage

### Setting up Gameplay Tags on an Enemy

```csharp
public class Enemy : MonoBehaviour
{
    [SerializeField] private TagContainer enemyTags = new TagContainer();
    
    private void Awake()
    {
        // Add tags programmatically if needed
        enemyTags.AddTag("Enemy.Melee.Aggressive");
    }
}
```

### Checking for Tag Matches in an Ability System

```csharp
public class AbilitySystem : MonoBehaviour
{
    [SerializeField] private TagContainer requiredTargetTags = new TagContainer();
    
    public bool CanApplyToTarget(GameObject target)
    {
        // Get the target's tag container component
        var targetTags = target.GetComponent<TagContainerComponent>();
        if (targetTags == null)
            return false;
            
        // Check if the target has all the required tags
        return targetTags.Container.MatchesAll(requiredTargetTags);
    }
}
```

## Example: Creating a Tag Container Component

For convenience, you may want to create a component to attach tag containers to GameObjects:

```csharp
using UnityEngine;
using HibokenGames.GameplayTags;

public class TagContainerComponent : MonoBehaviour
{
    [SerializeField] private TagContainer container = new TagContainer();
    
    public TagContainer Container => container;
    
    public bool HasTag(string tagName) => container.HasTag(tagName);
    
    public bool HasTagInHierarchy(string tagName) => container.HasTagInHierarchy(tagName);
    
    public void AddTag(string tagName) => container.AddTag(tagName);
    
    public bool RemoveTag(string tagName) => container.RemoveTag(tagName);
}
```

## Performance Considerations

- Tag comparisons are case-insensitive but cached internally for performance
- Re-registering the same tag returns the existing instance (no duplicates)
- The tag system uses string comparisons for hierarchical checking, which is efficient for most use cases

## License

This package is released under the [MIT License](LICENSE).

## Support

For support, bug reports, or feature requests, please open an issue on the [GitHub repository](https://github.com/hibokengames/gameplaytags/issues).