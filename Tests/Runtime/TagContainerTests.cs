using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HibokenGames.GameplayTags.Tests.Runtime
{
    public class TagContainerTests
    {
        private GameObject managerGO;
        private TagManager manager;

        [SetUp]
        public void Setup()
        {
            // Create a TagManager instance for the tests
            managerGO = new GameObject("TestTagManager");
            manager = managerGO.AddComponent<TagManager>();
            manager.ClearAllTags();
            
            // Register some test tags
            manager.RegisterTag("Enemy");
            manager.RegisterTag("Enemy.Melee");
            manager.RegisterTag("Enemy.Melee.Aggressive");
            manager.RegisterTag("Enemy.Ranged");
            manager.RegisterTag("Player");
            manager.RegisterTag("Player.Warrior");
            manager.RegisterTag("Item");
            manager.RegisterTag("Item.Weapon");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(managerGO);
        }

        [Test]
        public void TagContainer_EmptyConstructor_CreatesEmptyContainer()
        {
            // Arrange & Act
            var container = new TagContainer();
            
            // Assert
            Assert.AreEqual(0, container.Tags.Count);
        }

        [Test]
        public void TagContainer_Construction_WithInitialTags()
        {
            // Arrange & Act
            var container = new TagContainer(new[] { "Enemy", "Player" });
            
            // Assert
            Assert.AreEqual(2, container.Tags.Count);
            Assert.IsTrue(container.HasTag("Enemy"));
            Assert.IsTrue(container.HasTag("Player"));
        }

        [Test]
        public void TagContainer_AddTag_AddsTag()
        {
            // Arrange
            var container = new TagContainer();
            
            // Act
            container.AddTag("Enemy");
            
            // Assert
            Assert.AreEqual(1, container.Tags.Count);
            Assert.IsTrue(container.HasTag("Enemy"));
        }

        [Test]
        public void TagContainer_AddTag_IgnoresDuplicates()
        {
            // Arrange
            var container = new TagContainer();
            container.AddTag("Enemy");
            
            // Act
            container.AddTag("Enemy");
            container.AddTag("enemy"); // Case difference should be ignored
            
            // Assert
            Assert.AreEqual(1, container.Tags.Count);
        }

        [Test]
        public void TagContainer_AddTags_AddMultipleTags()
        {
            // Arrange
            var container = new TagContainer();
            
            // Act
            container.AddTags(new[] { "Enemy", "Player", "Item" });
            
            // Assert
            Assert.AreEqual(3, container.Tags.Count);
            Assert.IsTrue(container.HasTag("Enemy"));
            Assert.IsTrue(container.HasTag("Player"));
            Assert.IsTrue(container.HasTag("Item"));
        }

        [Test]
        public void TagContainer_RemoveTag_RemovesTag()
        {
            // Arrange
            var container = new TagContainer(new[] { "Enemy", "Player" });
            
            // Act
            bool result = container.RemoveTag("Enemy");
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, container.Tags.Count);
            Assert.IsFalse(container.HasTag("Enemy"));
            Assert.IsTrue(container.HasTag("Player"));
        }

        [Test]
        public void TagContainer_RemoveTag_WithNonexistentTag_ReturnsFalse()
        {
            // Arrange
            var container = new TagContainer(new[] { "Enemy" });
            
            // Act
            bool result = container.RemoveTag("Player");
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, container.Tags.Count);
        }

        [Test]
        public void TagContainer_HasTag_ReturnsTrueForExistingTags()
        {
            // Arrange
            var container = new TagContainer(new[] { "Enemy", "Player" });
            
            // Act & Assert
            Assert.IsTrue(container.HasTag("Enemy"));
            Assert.IsTrue(container.HasTag("Player"));
            Assert.IsFalse(container.HasTag("Item"));
        }

        [Test]
        public void TagContainer_HasTagInHierarchy_ReturnsTrueForTagsInHierarchy()
        {
            // Arrange
            var container = new TagContainer(new[] { "Enemy.Melee" });
            
            // Act & Assert
            Assert.IsTrue(container.HasTagInHierarchy("Enemy"));
            Assert.IsTrue(container.HasTagInHierarchy("Enemy.Melee"));
            Assert.IsTrue(container.HasTagInHierarchy("Enemy.Melee.Aggressive"));
            Assert.IsFalse(container.HasTagInHierarchy("Player"));
        }

        [Test]
        public void TagContainer_MatchesAny_ReturnsTrueWhenAnyTagMatches()
        {
            // Arrange
            var container1 = new TagContainer(new[] { "Enemy.Melee", "Item" });
            var container2 = new TagContainer(new[] { "Player", "Item.Weapon" });
            var container3 = new TagContainer(new[] { "Player.Warrior" });
            
            // Act & Assert
            Assert.IsTrue(container1.MatchesAny(container2), "Should match on Item/Item.Weapon hierarchy");
            Assert.IsFalse(container1.MatchesAny(container3), "No matching tags between containers");
        }

        [Test]
        public void TagContainer_MatchesAll_ReturnsTrueWhenAllTagsMatch()
        {
            // Arrange
            var container1 = new TagContainer(new[] { "Enemy", "Item" });
            var container2 = new TagContainer(new[] { "Enemy" });
            var container3 = new TagContainer(new[] { "Enemy", "Player" });
            
            // Act & Assert
            Assert.IsTrue(container1.MatchesAll(container2), "Container1 has all tags from Container2");
            Assert.IsFalse(container1.MatchesAll(container3), "Container1 doesn't have Player tag");
            Assert.IsTrue(container2.MatchesAll(new TagContainer()), "Empty container should match all");
            Assert.IsTrue(new TagContainer().MatchesAll(new TagContainer()), "Empty containers match all");
            Assert.IsFalse(new TagContainer().MatchesAll(container1), "Empty container doesn't match non-empty");
        }

        [Test]
        public void TagContainer_ClearTags_RemovesAllTags()
        {
            // Arrange
            var container = new TagContainer(new[] { "Enemy", "Player", "Item" });
            
            // Act
            container.ClearTags();
            
            // Assert
            Assert.AreEqual(0, container.Tags.Count);
            Assert.IsFalse(container.HasTag("Enemy"));
            Assert.IsFalse(container.HasTag("Player"));
            Assert.IsFalse(container.HasTag("Item"));
        }
    }
}