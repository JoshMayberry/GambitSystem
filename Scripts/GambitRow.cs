using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

using jmayberry.Spawner;

namespace jmayberry.GambitSystem {
	public interface IGambitContext {
	}

	[System.Serializable]
	public abstract class GambitRow<C,A> : ISpawnable where C : Enum where A : Enum {
		public bool isEnabled;
		public bool isLinked; // Used to create multi-condition logic
		public C condition;
		public A action;

        public GambitRow() {
        }

        public GambitRow(C condition, A action) {
            this.condition = condition;
            this.action = action;
        }

        public GambitRow(C condition, A action, bool isEnabled, bool isLinked) {
			this.isEnabled = isEnabled;
			this.isLinked = isLinked;
			this.condition = condition;
			this.action = action;
		}

		public GambitRow(GambitRow<C,A> duplicateFrom) {
			this.isEnabled = duplicateFrom.isEnabled;
			this.isLinked = duplicateFrom.isLinked;
			this.condition = duplicateFrom.condition;
			this.action = duplicateFrom.action;
		}

		// Part of the ISpawnable interface; called when a new row is populated
		public void OnSpawn(object spawner) {
		}

		// Part of the ISpawnable interface; called when a new row is not currently needed
		public void OnDespawn(object spawner) {
		}

		public abstract GambitRow<C, A> CreateDuplicate();

		// When implemented, this should have a switch case that checks what to do based on which enum was selected by the conditionSelector
		public abstract bool CheckCondition(IGambitContext context);

		// When implemented, this should have a switch case that does an action based on which enum was selected by the actionSelector
		public abstract void Execute(IGambitContext context);

		public bool EvaluateGambit(IGambitContext context) {
			if (!this.isEnabled) {
				return false;
			}

			if (!this.CheckCondition(context)) {
				return false;
			}

			if (isLinked) {
				return true;
			}

			this.Execute(context);
			return true;
		}

		public void Clear() {
			this.isEnabled = false;
			this.isLinked = false;
			this.condition = (C)(object)(0);
			this.action = (A)(object)(0);
		}
	}

    // Attach this to the prefab that represents a single row in the gambit UI
    public abstract class GambitUIRow<C,A> : MonoBehaviour, ISpawnable where C : Enum where A : Enum {
		public Dropdown conditionSelector;
		public Dropdown actionSelector;
		public Button enableButton;

		private GambitRow<C,A> rowData;
		internal UnityEvent OnRowModified; // Used to add a new row to the UI if this was the last row
	   
		public void SetRowData(GambitRow<C, A> rowData) {
			this.rowData = rowData;
			this.UpdateUIFromData();
		}

		private void UpdateUIFromData() {
			this.enableButton.GetComponentInChildren<Text>().text = (this.rowData.isEnabled ? "On" : "Off");
			this.conditionSelector.value = (int)(object)this.rowData.condition;
			this.actionSelector.value = (int)(object)this.rowData.action;
		}

		public void OnSpawn(object spawner) {
			this.conditionSelector.gameObject.SetActive(true);
			this.actionSelector.gameObject.SetActive(true);
			this.enableButton.gameObject.SetActive(true);

			this.conditionSelector.onValueChanged.AddListener(this.OnConditionModified);
			this.actionSelector.onValueChanged.AddListener(this.OnActionModified);
			this.enableButton.onClick.AddListener(this.OnEnableModified);
		}

		public void OnDespawn(object spawner) {
			this.conditionSelector.gameObject.SetActive(false);
			this.actionSelector.gameObject.SetActive(false);
			this.enableButton.gameObject.SetActive(false);

			this.conditionSelector.onValueChanged.RemoveListener(this.OnConditionModified);
			this.actionSelector.onValueChanged.RemoveListener(this.OnActionModified);
			this.enableButton.onClick.RemoveListener(this.OnEnableModified);
		}

		private void OnConditionModified(int value) {
			this.rowData.condition = (C)(object)value;
			this.OnRowModified.Invoke();
		}

		private void OnActionModified(int value) {
			this.rowData.action = (A)(object)value;
			this.OnRowModified.Invoke();
		}

		private void OnEnableModified() {
			this.rowData.isEnabled = !this.rowData.isEnabled;
			this.enableButton.GetComponentInChildren<Text>().text = (this.rowData.isEnabled ? "On" : "Off");
		}

        // Should call the gambit manager's copy method
        public abstract void Copy(GambitManagerBase<C, A> gambitManager);

        // Should call the gambit manager's copy method
        public abstract void Paste(GambitManagerBase<C, A> gambitManager);

        // Should call the gambit manager's copy method
        public abstract void Duplicate(GambitManagerBase<C, A> gambitManager);

        public void Clear() {
			this.rowData.Clear();
			this.UpdateUIFromData();
		}
	}

	public abstract class GambitManagerBase<C,A> : MonoBehaviour where C : Enum where A : Enum {
		public GameObject rowContainerRoot; // Could be a ScrollView
		public GameObject rowContainer; // Could be a VerticalLayoutGroup
		public GambitUIRow<C,A> rowPrefab;

		private UnitySpawner<GambitUIRow<C,A>> rowList;
		private static GambitRow<C,A> clipboardRowData;

		public static GambitManagerBase<C,A> instance { get; private set; }
		private void Awake() {
			if (instance != null) {
				Debug.LogError("Found more than one GambitManager in the scene.");
			}

			instance = this;

			this.rowList = new UnitySpawner<GambitUIRow<C,A>>();
		}

		private void Start() {
			this.rowList.SetPrefabDefault(this.rowPrefab);
		}

		public void ViewGambits(ref List<GambitRow<C,A>> gambitRows) {
			// Show a container that has a scrollable list of rows populated with gambitRows.
			// When things are edited on the UI, it should mutate the gambitRows list

			// Populate the UI list with the gambit rows
			this.rowList.DespawnAll();

			foreach (GambitRow<C, A> row in gambitRows) {
				GambitUIRow<C, A> rowUi = this.rowList.Spawn(Vector3.zero, this.rowContainer.transform);
				rowUi.SetRowData(row);
			}


			// Ensure the container that holds the gambit list is visible
			if (this.rowContainerRoot != null) {
				this.rowContainerRoot.SetActive(true);
			}
		}

		public static void CopyToClipboard(GambitRow<C,A> data) {
			clipboardRowData = data;
		}

		public static GambitRow<C,A> GetClipboardData() {
			return clipboardRowData;
		}

		public void Paste(ref List<GambitRow<C, A>> gambitRows, int index) {
			if (clipboardRowData == null) {
				return;
			}

            gambitRows.Insert(index, clipboardRowData.CreateDuplicate());
            this.ViewGambits(ref gambitRows); // Refresh GUI
        }

        public void Duplicate(ref List<GambitRow<C, A>> gambitRows, int index) {
            GambitRow<C, A> duplicateThis = gambitRows[index];
            gambitRows.Insert(index, duplicateThis.CreateDuplicate());
            this.ViewGambits(ref gambitRows); // Refresh GUI
        }

        public void Remove(ref List<GambitRow<C, A>> gambitRows, int index) {
            gambitRows.RemoveAt(index);
            this.ViewGambits(ref gambitRows); // Refresh GUI
        }

        public void Clear(ref List<GambitRow<C, A>> gambitRows, int index) {
            GambitRow<C, A> row = gambitRows[index];
            row.Clear();
            this.ViewGambits(ref gambitRows); // Refresh GUI
        }
    }
}