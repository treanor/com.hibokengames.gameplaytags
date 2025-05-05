using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HibokenGames.GameplayTags.Editor
{
    /// <summary>
    /// Editor window for viewing and managing gameplay tags.
    /// </summary>
    public class TagManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string newTagName = string.Empty;
        private string searchFilter = string.Empty;
        private List<Tag> registeredTags = new List<Tag>();
        private Dictionary<string, bool> expandedCategories = new Dictionary<string, bool>();

        [MenuItem("Window/Hiboken Games/Gameplay Tags")]
        public static void ShowWindow()
        {
            GetWindow<TagManagerWindow>("Gameplay Tags");
        }

        private void OnEnable()
        {
            // Auto-refresh when the window is enabled
            RefreshTagList();
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space();
            DrawSearchBar();
            EditorGUILayout.Space();
            DrawTagList();
            EditorGUILayout.Space();
            DrawAddTagSection();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    RefreshTagList();
                }
                
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("Expand All", EditorStyles.toolbarButton, GUILayout.Width(70)))
                {
                    foreach (var category in GetTopLevelCategories())
                    {
                        expandedCategories[category] = true;
                    }
                }
                
                if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    foreach (var category in GetTopLevelCategories())
                    {
                        expandedCategories[category] = false;
                    }
                }
            }
        }

        private void DrawSearchBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Filter:", GUILayout.Width(40));
                string newFilter = EditorGUILayout.TextField(searchFilter);
                
                if (newFilter != searchFilter)
                {
                    searchFilter = newFilter;
                    RefreshTagList();
                }
                
                if (GUILayout.Button("Clear", GUILayout.Width(50)))
                {
                    searchFilter = string.Empty;
                    RefreshTagList();
                }
            }
        }

        private void DrawTagList()
        {
            EditorGUILayout.LabelField("Registered Tags", EditorStyles.boldLabel);
            
            if (registeredTags.Count == 0)
            {
                EditorGUILayout.HelpBox("No tags registered. Add tags using the field below.", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Group tags by top-level category
            var tagsByCategory = GroupTagsByCategory();
            
            foreach (var categoryKvp in tagsByCategory)
            {
                string category = categoryKvp.Key;
                List<Tag> tagsInCategory = categoryKvp.Value;
                
                // Ensure the category has an entry in our expanded dictionary
                if (!expandedCategories.ContainsKey(category))
                {
                    expandedCategories[category] = true;
                }
                
                // Draw the category foldout
                EditorGUILayout.BeginHorizontal();
                expandedCategories[category] = EditorGUILayout.Foldout(expandedCategories[category], category, true);
                EditorGUILayout.EndHorizontal();
                
                // If expanded, show the tags in this category
                if (expandedCategories[category])
                {
                    EditorGUI.indentLevel++;
                    foreach (var tag in tagsInCategory)
                    {
                        DrawTagEntry(tag);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTagEntry(Tag tag)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tag.Name);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddTagSection()
        {
            EditorGUILayout.LabelField("Add New Tag", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            newTagName = EditorGUILayout.TextField(newTagName);
            
            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(newTagName));
            if (GUILayout.Button("Add Tag", GUILayout.Width(80)))
            {
                if (!string.IsNullOrWhiteSpace(newTagName))
                {
                    AddNewTag(newTagName);
                    newTagName = string.Empty;
                    RefreshTagList();
                    GUI.FocusControl(null); // Clear focus to reset the control
                }
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox(
                "Tags should use dot notation for hierarchy (e.g., 'Enemy.Melee.Aggressive').", 
                MessageType.Info);
        }

        private void RefreshTagList()
        {
            // At runtime, we can access the TagManager instance
            if (Application.isPlaying)
            {
                registeredTags = new List<Tag>(TagManager.Instance.GetAllTags());
            }
            else
            {
                // During edit mode, we can't directly access the runtime TagManager
                // You might want to implement a way to persist and load tags in edit mode
                registeredTags = new List<Tag>();
            }

            // Filter tags based on search text
            if (!string.IsNullOrWhiteSpace(searchFilter))
            {
                registeredTags = registeredTags
                    .Where(t => t.Name.IndexOf(searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            // Ensure tag list is sorted
            registeredTags = registeredTags.OrderBy(t => t.Name).ToList();
            
            Repaint();
        }

        private void AddNewTag(string tagName)
        {
            if (Application.isPlaying)
            {
                TagManager.Instance.RegisterTag(tagName);
            }
            else
            {
                // During edit mode, implement storing tags between play sessions
                // This could be done via EditorPrefs, ScriptableObject, or other persistence mechanism
                // For now, just show a message
                Debug.Log($"Tag '{tagName}' would be registered at runtime. In edit mode, tag persistence would require additional implementation.");
            }
        }

        private Dictionary<string, List<Tag>> GroupTagsByCategory()
        {
            var result = new Dictionary<string, List<Tag>>();
            
            foreach (var tag in registeredTags)
            {
                string topCategory = GetTopCategory(tag.Name);
                
                if (!result.ContainsKey(topCategory))
                {
                    result[topCategory] = new List<Tag>();
                }
                
                result[topCategory].Add(tag);
            }
            
            // Sort the dictionary by key
            return result.OrderBy(kvp => kvp.Key)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private string GetTopCategory(string tagName)
        {
            int dotIndex = tagName.IndexOf('.');
            return dotIndex > 0 ? tagName.Substring(0, dotIndex) : tagName;
        }

        private List<string> GetTopLevelCategories()
        {
            return registeredTags
                .Select(t => GetTopCategory(t.Name))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }
    }
}