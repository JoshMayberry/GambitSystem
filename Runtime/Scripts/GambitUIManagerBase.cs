using UnityEngine;
using System;

using jmayberry.Spawner;

namespace jmayberry.GambitSystem {
	public interface IGambitUIManager { }

	public abstract class GambitUIManagerBase : MonoBehaviour, IGambitUIManager {
		public GameObject rowContainerRoot; // Could be a ScrollView
		public GameObject rowContainer; // Could be a VerticalLayoutGroup
		public GambitUIRow uiRowPrefab;
		public UnitySpawner<GambitUIRow> rowList;

		// Can use either currentGambitRowList or currentCharacter
		//public GambitRowList currentGambitRowList;
		public IGambitCharacter currentCharacter;

		private static string clipboardRowData;

		public static GambitUIManagerBase instance { get; private set; }
		private void Awake() {
			if (instance != null) {
				Debug.LogError("Found more than one GambitManager in the scene.");
			}

			instance = this;

			this.rowList = new UnitySpawner<GambitUIRow>();
		}

		private void Start() {
			this.rowList.SetPrefabDefault(this.uiRowPrefab);
		}

		//public void SetGambitList(GambitRowList rowList) {
		//	this.currentGambitRowList = rowList;
		//	this.currentCharacter = null;
		//}

		//public void SetCharacter(IGambitCharacter character) {
		//	this.currentCharacter = character;
		//	this.currentGambitRowList = character.GetGambitRowList();
		//}


		//public void ViewGambits(GambitRowList rowList) {
		//	this.SetGambitList(rowList);
		//	this.ViewGambits();
		//}

		//public void ViewGambits(IGambitCharacter character) {
		//	this.SetCharacter(character);
		//	this.ViewGambits();
		//}


		//public void ViewGambits() {
		//	// Show a container that has a scrollable list of rows populated with gambitRows.
		//	// When things are edited on the UI, it should mutate the gambitRows list

		//	// Populate the UI list with the gambit rows
		//	this.rowList.DespawnAll();

		//	if (this.currentGambitRowList == null) {
		//		Debug.LogError("No row list was selected");
		//		return;
		//	}

		//	foreach (GambitRow row in this.currentGambitRowList) {
		//		GambitUIRow rowUi = this.rowList.Spawn(Vector3.zero, this.rowContainer.transform);
		//		rowUi.SetRowData(row);
		//	}

		//	// Ensure the container that holds the gambit list is visible
		//	if (this.rowContainerRoot != null) {
		//		this.rowContainerRoot.SetActive(true);
		//	}
		//}

		//public static void CopyToClipboard(GambitRow row) {
		//	clipboardRowData = row.ToJSON();
		//}

		public static string GetClipboardData() {
			return clipboardRowData;
		}
	}
}