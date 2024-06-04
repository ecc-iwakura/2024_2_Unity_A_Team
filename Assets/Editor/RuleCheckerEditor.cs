using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(RuleChecker))]
public class RuleCheckerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RuleChecker ruleChecker = (RuleChecker)target;

        if (ruleChecker.availableRules == null) return;

        List<string> ruleNames = new List<string>();
        foreach (var rule in ruleChecker.availableRules)
        {
            ruleNames.Add(rule.ruleName);
        }

        // Add Rule Button
        if (GUILayout.Button("Add Rule"))
        {
            ruleChecker.selectedRules.Add(new RuleChecker.RuleReference());
            EditorUtility.SetDirty(ruleChecker); // Mark the ruleChecker as dirty to save changes
        }

        for (int i = 0; i < ruleChecker.selectedRules.Count; i++)
        {
            int index = ruleNames.IndexOf(ruleChecker.selectedRules[i].ruleName);
            index = EditorGUILayout.Popup("Rule", index, ruleNames.ToArray());
            if (index >= 0 && index < ruleNames.Count)
            {
                ruleChecker.selectedRules[i].ruleName = ruleNames[index];
            }
        }

        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ruleChecker);
        }
    }
}