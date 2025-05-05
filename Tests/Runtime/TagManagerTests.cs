using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HibokenGames.GameplayTags.Tests.Runtime
{
    public class TagManagerTests
    {
        private GameObject managerGO;
        private TagManager manager;

        [SetUp]
        public void Setup()
        {
            // Create a fresh instance for each test
            managerGO = new GameObject("TestTagManager");
            manager = managerGO.AddComponent<TagManager>();
            
            // Clear any existing tags
            manager.ClearAllTags();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(managerGO);
        }

        [Test]
        public void TagManager_CanRegisterTag()
        {
            // Arrange & Act
            var tag = manager.RegisterTag("Test.Tag");
            
            // Assert
            Assert.IsNotNull(tag);
            Assert.AreEqual("Test.Tag", tag.Name);
            Assert.IsTrue(manager.HasTag("Test.Tag"));
        }

        [Test]
        public void TagManager_RegisteringSameTag_ReturnsSameInstance()
        {
            // Arrange
            var tag1 = manager.RegisterTag("Test.Tag");
            
            // Act
            var tag2 = manager.RegisterTag("Test.Tag");
            
            // Assert
            Assert.AreEqual(tag1, tag2);
            Assert.IsTrue(ReferenceEquals(tag1, tag2), "Tags should be the same instance");
        }

        [Test]
        public void TagManager_GetTag_ReturnsCorrectTag()
        {
            // Arrange
            var originalTag = manager.RegisterTag("Test.Tag");
            
            // Act
            var retrievedTag = manager.GetTag("Test.Tag");
            
            // Assert
            Assert.IsNotNull(retrievedTag);
            Assert.AreEqual(originalTag, retrievedTag);
        }

        [Test]
        public void TagManager_GetTag_WithUnknownTag_ReturnsNull()
        {
            // Act
            var retrievedTag = manager.GetTag("Unknown.Tag");
            
            // Assert
            Assert.IsNull(retrievedTag);
        }

        [Test]
        public void TagManager_HasTag_WorksCorrectly()
        {
            // Arrange
            manager.RegisterTag("Test.Tag");
            
            // Act & Assert
            Assert.IsTrue(manager.HasTag("Test.Tag"));
            Assert.IsFalse(manager.HasTag("Unknown.Tag"));
        }

        [Test]
        public void TagManager_GetAllTags_ReturnsAllRegisteredTags()
        {
            // Arrange
            var tag1 = manager.RegisterTag("Tag1");
            var tag2 = manager.RegisterTag("Tag2");
            var tag3 = manager.RegisterTag("Tag3");
            
            // Act
            var allTags = manager.GetAllTags();
            
            // Assert
            Assert.AreEqual(3, allTags.Count);
            Assert.IsTrue(allTags.Contains(tag1));
            Assert.IsTrue(allTags.Contains(tag2));
            Assert.IsTrue(allTags.Contains(tag3));
        }

        [Test]
        public void TagManager_FindChildTags_ReturnsCorrectTags()
        {
            // Arrange
            manager.RegisterTag("Enemy");
            var meleeTag = manager.RegisterTag("Enemy.Melee");
            var aggressiveTag = manager.RegisterTag("Enemy.Melee.Aggressive");
            var rangedTag = manager.RegisterTag("Enemy.Ranged");
            manager.RegisterTag("Player");
            
            // Act
            var childTags = manager.FindChildTags("Enemy");
            
            // Assert
            Assert.AreEqual(3, childTags.Count);
            Assert.IsTrue(childTags.Contains(meleeTag));
            Assert.IsTrue(childTags.Contains(aggressiveTag));
            Assert.IsTrue(childTags.Contains(rangedTag));
        }

        [Test]
        public void TagManager_Singleton_ReturnsInstance()
        {
            // Act
            var instance = TagManager.Instance;
            
            // Assert
            Assert.IsNotNull(instance);
        }

        [Test]
        public void TagManager_ClearAllTags_RemovesAllTags()
        {
            // Arrange
            manager.RegisterTag("Tag1");
            manager.RegisterTag("Tag2");
            
            // Act
            manager.ClearAllTags();
            
            // Assert
            Assert.AreEqual(0, manager.GetAllTags().Count);
            Assert.IsFalse(manager.HasTag("Tag1"));
            Assert.IsFalse(manager.HasTag("Tag2"));
        }
    }
}