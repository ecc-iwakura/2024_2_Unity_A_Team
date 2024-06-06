using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(RuleChecker))]
public class RuleCheckerEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        RuleChecker ruleChecker = (RuleChecker)target;

        reorderableList = new ReorderableList(ruleChecker.selectedRules, typeof(RuleChecker.RuleReference), true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x + 15, rect.y, rect.width - 15, rect.height), "Selected Rules", EditorStyles.boldLabel);
        };


        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = ruleChecker.selectedRules[index];
            List<string> ruleNames = new List<string>();
            foreach (var rule in ruleChecker.availableRules)
            {
                ruleNames.Add(rule.ruleName);
            }

            rect.y += 2;
            int selectedIndex = ruleNames.IndexOf(element.ruleName);
            selectedIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), selectedIndex, ruleNames.ToArray());
            if (selectedIndex >= 0 && selectedIndex < ruleNames.Count)
            {
                element.ruleName = ruleNames[selectedIndex];
            }
        };

        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            ruleChecker.selectedRules.Add(new RuleChecker.RuleReference());
            EditorUtility.SetDirty(ruleChecker); // Mark the ruleChecker as dirty to save changes
        };


        ruleChecker.InitializeRules();
    }

    public override void OnInspectorGUI()
    {
        RuleChecker ruleChecker = (RuleChecker)target;

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Rule Checker", ruleChecker, typeof(RuleChecker), true);
        EditorGUI.EndDisabledGroup();
        EditorGUI.indentLevel--;

        // 元のフィールドを表示
        DrawDefaultInspector();

        if (ruleChecker.availableRules == null) return;

        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}