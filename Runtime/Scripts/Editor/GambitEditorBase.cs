using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace jmayberry.GambitSystem.Editor {

	public abstract class GambitEditorBase<T, C, A> : UnityEditor.Editor where T : MonoBehaviour where C : Enum where A : Enum {
		protected T targetObject;

		private void OnEnable() {
			targetObject = (T)target;
		}

		protected abstract void InitializeGambitRowList();

		public abstract IGambitRowList GetRowList();

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			IGambitRowList rowList = this.GetRowList();

			if (rowList == null) {
				EditorGUILayout.HelpBox("Gambit List is not initialized.", MessageType.Warning);
				if (GUILayout.Button("Initialize Gambit List")) {
					this.InitializeGambitRowList();
					EditorUtility.SetDirty(targetObject);
				}

				return;
			}

			EditorGUI.BeginChangeCheck();

			List<IGambitRow> removeList = new List<IGambitRow>();

			int i = 0;
			foreach (var gambitRow in rowList) {
				EditorGUILayout.BeginVertical("box");
				EditorGUILayout.LabelField($"Gambit Row {i + 1}", EditorStyles.boldLabel);

				gambitRow.SetCondition(EditorGUILayout.EnumPopup("Condition", gambitRow.GetCondition()));
				gambitRow.SetAction(EditorGUILayout.EnumPopup("Action", gambitRow.GetAction()));

				gambitRow.SetIsLinked(EditorGUILayout.Toggle("Is Linked", gambitRow.GetIsLinked()));
				gambitRow.SetIsEnabled(EditorGUILayout.Toggle("Is Enabled", gambitRow.GetIsEnabled()));

				if (GUILayout.Button("Remove")) {
                    removeList.Add(gambitRow);
				}

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();

				i++;
			}

			foreach (var gambitRow in removeList) {
                rowList.Remove(gambitRow);
            }

            if (GUILayout.Button("Add New Gambit")) {
				rowList.AddEmpty();

				EditorUtility.SetDirty(targetObject);
			}

			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(targetObject);
			}
		}
	}
}