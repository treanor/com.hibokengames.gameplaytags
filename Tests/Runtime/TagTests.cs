using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HibokenGames.GameplayTags.Tests.Runtime
{
    public class TagTests
    {
        [Test]
        public void Tag_Creation_SetsNameCorrectly()
        {
            // Arrange & Act
            const string tagName = "Test.Tag.Name";
            var tag = new Tag(tagName);
            
            // Assert
            Assert.AreEqual(tagName, tag.Name);
        }
        
        [Test]
        public void Tag_WithEmptyName_ThrowsException()
        {
            // Assert
            Assert.Throws<System.ArgumentException>(() => new Tag(""));
            Assert.Throws<System.ArgumentException>(() => new Tag(null));
            Assert.Throws<System.ArgumentException>(() => new Tag("   "));
        }
        
        [Test]
        public void Tag_WithWhitespace_TrimsName()
        {
            // Arrange & Act
            const string tagName = "Test.Tag";
            var tag = new Tag($"  {tagName}  ");
            
            // Assert
            Assert.AreEqual(tagName, tag.Name);
        }
        
        [Test]
        public void Tag_Equality_WorksCorrectly()
        {
            // Arrange
            var tag1 = new Tag("Test.Tag");
            var tag2 = new Tag("Test.Tag");
            var tag3 = new Tag("Other.Tag");
            
            // Assert
            Assert.AreEqual(tag1, tag2);
            Assert.AreNotEqual(tag1, tag3);
            Assert.IsTrue(tag1 == tag2);
            Assert.IsFalse(tag1 == tag3);
            Assert.IsFalse(tag1 != tag2);
            Assert.IsTrue(tag1 != tag3);
        }
        
        [Test]
        public void Tag_Equality_IsCaseInsensitive()
        {
            // Arrange
            var tag1 = new Tag("Test.Tag");
            var tag2 = new Tag("TEST.tag");
            
            // Assert
            Assert.AreEqual(tag1, tag2);
            Assert.IsTrue(tag1 == tag2);
        }
        
        [Test]
        public void Tag_IsParentOf_WorksCorrectly()
        {
            // Arrange
            var parent = new Tag("Enemy");
            var child = new Tag("Enemy.Melee");
            var grandchild = new Tag("Enemy.Melee.Aggressive");
            var unrelated = new Tag("Player");
            
            // Assert - Parent cases
            Assert.IsTrue(parent.IsParentOf(parent), "A tag should be its own parent");
            Assert.IsTrue(parent.IsParentOf(child), "Parent should be parent of direct child");
            Assert.IsTrue(parent.IsParentOf(grandchild), "Parent should be parent of grandchild");
            Assert.IsFalse(parent.IsParentOf(unrelated), "Unrelated tags should not be parents");
            
            // Assert - Child cases
            Assert.IsTrue(grandchild.IsChildOf(parent), "Grandchild should be child of parent");
            Assert.IsTrue(child.IsChildOf(parent), "Child should be child of parent");
            Assert.IsFalse(parent.IsChildOf(child), "Parent should not be child of child");
            Assert.IsFalse(grandchild.IsChildOf(unrelated), "Unrelated tags should not be children");
        }
    }
}