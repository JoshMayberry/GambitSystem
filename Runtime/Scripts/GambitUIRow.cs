using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

using jmayberry.Spawner;

namespace jmayberry.GambitSystem {
	// Attach this to the prefab that represents a single row in the gambit UI
	public abstract class GambitUIRow<C, A> : MonoBehaviour, ISpawnable where C : Enum where A : Enum {
		public Dropdown conditionSelector;
		public Dropdown actionSelector;
		public Button enableButton;

		private IGambitRow rowData;
		internal UnityEvent OnRowModified; // Used to add a new row to the UI if this was the last row

		public void SetRowData(IGambitRow rowData) {
			this.rowData = rowData;
			this.UpdateUIFromData();
		}

		private void UpdateUIFromData() {
			this.enableButton.GetComponentInChildren<Text>().text = (this.rowData.GetIsEnabled() ? "On" : "Off");
			this.conditionSelector.value = (int)(object)this.rowData.GetCondition();
			this.actionSelector.value = (int)(object)this.rowData.GetAction();
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
			this.rowData.SetCondition((C)(object)value);
			this.OnRowModified.Invoke();
		}

		private void OnActionModified(int value) {
			this.rowData.SetAction((A)(object)value);
			this.OnRowModified.Invoke();
		}

		private void OnEnableModified() {
			bool value = !this.rowData.GetIsEnabled();
			this.rowData.SetIsEnabled(value);
			this.enableButton.GetComponentInChildren<Text>().text = (value ? "On" : "Off");
		}

		public void Clear() {
			this.rowData.Clear();
			this.UpdateUIFromData();
		}
	}
}