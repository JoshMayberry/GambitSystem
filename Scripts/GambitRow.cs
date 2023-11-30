using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

using jmayberry.Spawner;

namespace jmayberry.GambitSystem {
	[System.Serializable]
	public class GambitRowData<C,A> where C : Enum where A : Enum {
		public bool isEnabled;
		public C condition;
		public A action;
	}

	public interface IGambitContext {
	}

	public abstract class GambitRow<C,A> : ISpawnable where C : Enum where A : Enum {
		public GambitRowData<C, A> rowData;
		public Dropdown conditionSelector;
		public Dropdown actionSelector;
		public Button enableButton;

		internal UnityEvent OnRowModified; // Used to add a new row to the UI if this was the last row

        public GambitRow(Dropdown conditionSelector, Dropdown actionSelector, Button enableButton) {
            this.Initialize(conditionSelector, actionSelector, enableButton);
        }
        public GambitRow(Dropdown conditionSelector, Dropdown actionSelector, Button enableButton, GambitRowData<C, A> rowData) {
            this.Initialize(conditionSelector, actionSelector, enableButton, rowData);
        }

        public void Initialize(Dropdown conditionSelector, Dropdown actionSelector, Button enableButton) {
			this.Initialize(conditionSelector, actionSelector, enableButton, null);
		}

		public void Initialize(Dropdown conditionSelector, Dropdown actionSelector, Button enableButton, GambitRowData<C,A> rowData) {
			this.conditionSelector = conditionSelector;
			this.actionSelector = actionSelector;
			this.enableButton = enableButton;
			this.rowData = rowData ?? new GambitRowData<C, A>();

			// Remove all previous listeners
			this.conditionSelector.onValueChanged.RemoveAllListeners();
			this.actionSelector.onValueChanged.RemoveAllListeners();
			this.enableButton.onClick.RemoveAllListeners();

			// Add listeners
			this.conditionSelector.onValueChanged.AddListener(this.OnConditionModified);
			this.actionSelector.onValueChanged.AddListener(OnActionModified);
			this.enableButton.onClick.AddListener(OnEnableModified);
		}

		public void UnInitialize() {
			this.conditionSelector.onValueChanged.RemoveAllListeners();
			this.actionSelector.onValueChanged.RemoveAllListeners();
			this.enableButton.onClick.RemoveAllListeners();
		}

		// Part of the ISpawnable interface; called when a new row is populated
		public void OnSpawn(object spawner) {
			this.conditionSelector.gameObject.SetActive(true);
			this.actionSelector.gameObject.SetActive(true);
			this.enableButton.gameObject.SetActive(true);
		}

		// Part of the ISpawnable interface; called when a new row is not currently needed
		public void OnDespawn(object spawner) {
			this.conditionSelector.gameObject.SetActive(false);
			this.actionSelector.gameObject.SetActive(false);
			this.enableButton.gameObject.SetActive(false);
		}

		public void OnConditionModified(int value) {
			this.rowData.condition = (C)(object)value;
			this.OnRowModified.Invoke();
		}

		public void OnActionModified(int value) {
			this.rowData.action = (A)(object)value;
			this.OnRowModified.Invoke();
		}

		public void OnEnableModified() {
			this.rowData.isEnabled = !this.rowData.isEnabled;
			this.enableButton.GetComponentInChildren<Text>().text = (this.rowData.isEnabled ? "On" : "Off");
		}

		// When implemented, this should have a switch case that checks what to do based on which enum was selected by the conditionSelector
		public abstract bool CheckCondition(IGambitContext context);

		// When implemented, this should have a switch case that does an action based on which enum was selected by the actionSelector
		public abstract void Execute(IGambitContext context);
	}

	public class GambitManagerBase<C,A> : MonoBehaviour where C : Enum where A : Enum {
		public static GambitManagerBase<C,A> instance { get; private set; }
		private void Awake() {
			if (instance != null) {
				Debug.LogError("Found more than one GambitManager in the scene.");
			}

			instance = this;
		}

		public bool EvaluateGambits(IGambitContext context, List<GambitRow<C,A>> gambitRows) {
			foreach (var gambitRow in gambitRows) {
				if (gambitRow.rowData.isEnabled && gambitRow.CheckCondition(context)) {
					gambitRow.Execute(context);
					return true;
				}
			}

			return false;
		}
	}
}