using UnityEngine;
using System;

using jmayberry.Spawner;

namespace jmayberry.GambitSystem {
	public abstract class GambitUIManagerBase<C, A> : MonoBehaviour where C : Enum where A : Enum {
		public GameObject rowContainerRoot; // Could be a ScrollView
		public GameObject rowContainer; // Could be a VerticalLayoutGroup
		public GambitUIRow<C, A> uiRowPrefab;

		private UnitySpawner<GambitUIRow<C, A>> rowList;
		private static string clipboardRowData;

		public static GambitUIManagerBase<C, A> instance { get; private set; }
		private void Awake() {
			if (instance != null) {
				Debug.LogError("Found more than one GambitManager in the scene.");
			}

			instance = this;

			this.rowList = new UnitySpawner<GambitUIRow<C, A>>();
		}

		private void Start() {
			this.rowList.SetPrefabDefault(this.uiRowPrefab);
		}

		public void ViewGambits(IGambitRowList gambitRowList) {
			// Show a container that has a scrollable list of rows populated with gambitRows.
			// When things are edited on the UI, it should mutate the gambitRows list

			// Populate the UI list with the gambit rows
			this.rowList.DespawnAll();

			foreach (IGambitRow row in gambitRowList) {
				GambitUIRow<C, A> rowUi = this.rowList.Spawn(Vector3.zero, this.rowContainer.transform);
				rowUi.SetRowData(row);
			}


			// Ensure the container that holds the gambit list is visible
			if (this.rowContainerRoot != null) {
				this.rowContainerRoot.SetActive(true);
			}
		}

		public static void CopyToClipboard(IGambitRow row) {
			clipboardRowData = row.ToJSON();
		}

		public static string GetClipboardData() {
			return clipboardRowData;
		}
	}
}