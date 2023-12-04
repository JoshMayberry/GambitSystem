using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using jmayberry.GambitSystem.Editor;
using jmayberry.GambitSystem;

[CustomEditor(typeof(AICharacter))]
public class AICharacterEditor : GambitEditorBase<AICharacter, MyGambitConditions, MyGambitCombatActions> {

	protected override void InitializeGambitRowList() {
		targetObject.myGambitRowList = new MyGambitRowList(new List<MyGambitRow>());
	}

	public override IGambitRowList GetRowList() {
		return targetObject.myGambitRowList;
	}
}
