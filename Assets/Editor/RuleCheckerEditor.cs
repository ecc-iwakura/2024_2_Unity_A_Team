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

            List<string> conditionNames = new List<string>();
            foreach (var condition in ruleChecker.availableConditions)
            {
                conditionNames.Add(condition.conditionName);
            }

            rect.y += 2;
            int conditionSelectedIndex = conditionNames.IndexOf(element.conditionName);
            conditionSelectedIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), conditionSelectedIndex, conditionNames.ToArray());
            if (conditionSelectedIndex >= 0 && conditionSelectedIndex < conditionNames.Count)
            {
                element.conditionName = conditionNames[conditionSelectedIndex];
            }

            element.actionFlag = (RuleChecker.ButtonFlag)EditorGUI.EnumPopup(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), element.actionFlag);
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
        EditorGUI.EndDisabledGroup();
        EditorGUI.indentLevel--;

        // 元のフィールドを表示
        DrawDefaultInspector();

        if (ruleChecker.availableConditions == null) return;

        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
