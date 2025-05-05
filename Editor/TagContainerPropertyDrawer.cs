using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using HibokenGames.GameplayTags;

namespace HibokenGames.GameplayTags.Editor
{
    /// <summary>
    /// Custom property drawer for TagContainer to make it more user-friendly in the Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(TagContainer))]
    public class TagContainerPropertyDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 60f;
        private bool isExpanded = true;
        private string newTag = string.Empty;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!isExpanded)
                return EditorGUIUtility.singleLineHeight;

            SerializedProperty tagNamesProperty = property.FindPropertyRelative("tagNames");
            int lineCount = tagNamesProperty.arraySize + 2; // List + header + add field
            
            return EditorGUIUtility.singleLineHeight * lineCount 
                + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw header
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            isExpanded = EditorGUI.Foldout(headerRect, isExpanded, label, true);

            if (isExpanded)
            {
                // Get the tags property
                SerializedProperty tagNamesProperty = property.FindPropertyRelative("tagNames");
                
                // Draw the tag list
                DrawTagList(position, tagNamesProperty);
                
                // Draw the add tag field
                DrawAddTagField(position, tagNamesProperty);
            }

            EditorGUI.EndProperty();
        }

        private void DrawTagList(Rect position, SerializedProperty tagNamesProperty)
        {
            // Start one line down from the header
            float yPos = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            for (int i = 0; i < tagNamesProperty.arraySize; i++)
            {
                SerializedProperty tagProp = tagNamesProperty.GetArrayElementAtIndex(i);
                
                Rect lineRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
                Rect tagRect = new Rect(lineRect.x, lineRect.y, lineRect.width - ButtonWidth - 4, lineRect.height);
                Rect buttonRect = new Rect(tagRect.xMax + 4, lineRect.y, ButtonWidth, lineRect.height);
                
                // Draw tag name field
                EditorGUI.BeginChangeCheck();
                string tagValue = EditorGUI.TextField(tagRect, tagProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    tagProp.stringValue = tagValue;
                }
                
                // Draw remove button
                if (GUI.Button(buttonRect, "Remove"))
                {
                    tagNamesProperty.DeleteArrayElementAtIndex(i);
                    break; // Break to avoid issues with the modified array
                }
                
                yPos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private void DrawAddTagField(Rect position, SerializedProperty tagNamesProperty)
        {
            // Position at the bottom of the control
            float yPos = position.y + EditorGUIUtility.singleLineHeight + 
                         (tagNamesProperty.arraySize + 1) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            
            Rect addRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
            Rect tagFieldRect = new Rect(addRect.x, addRect.y, addRect.width - ButtonWidth - 4, addRect.height);
            Rect addButtonRect = new Rect(tagFieldRect.xMax + 4, addRect.y, ButtonWidth, addRect.height);
            
            // Draw new tag field
            newTag = EditorGUI.TextField(tagFieldRect, newTag);
            
            // Draw add button
            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(newTag));
            if (GUI.Button(addButtonRect, "Add") && !string.IsNullOrWhiteSpace(newTag))
            {
                // Check if tag already exists
                bool tagExists = false;
                for (int i = 0; i < tagNamesProperty.arraySize; i++)
                {
                    if (string.Equals(tagNamesProperty.GetArrayElementAtIndex(i).stringValue, newTag, StringComparison.OrdinalIgnoreCase))
                    {
                        tagExists = true;
                        break;
                    }
                }
                
                if (!tagExists)
                {
                    tagNamesProperty.arraySize++;
                    tagNamesProperty.GetArrayElementAtIndex(tagNamesProperty.arraySize - 1).stringValue = newTag;
                    newTag = string.Empty; // Clear the field
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}