using UnityEngine;
using System;

using jmayberry.Spawner;


namespace jmayberry.GambitSystem {
	public interface IGambitRow {
		bool EvaluateGambit(IGambitContext context);

		void Clear();

		bool IsLinked();

		IGambitRow CreateDuplicate();

		string ToJSON();

		void FromJSON(string payload);

		void SetCondition(Enum selection);
		Enum GetCondition();

		void SetAction(Enum selection);
		Enum GetAction();

		void SetIsLinked(bool state);
		bool GetIsLinked();

		void SetIsEnabled(bool state);
		bool GetIsEnabled();
	}

	[System.Serializable]
	public abstract class GambitRow<C, A> : ISpawnable, IGambitRow where C : Enum where A : Enum {
		public bool isEnabled;
		public bool isLinked; // Used to create multi-condition logic
		public C condition;
		public A action;

		public GambitRow() { }

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
		public GambitRow(string jsonPayload) {
			this.FromJSON(jsonPayload);
		}

		public bool IsLinked() {
			return this.isLinked;
		}

		public string ToJSON() {
			return JsonUtility.ToJson(new {
				isEnabled = this.isEnabled,
				isLinked = this.isLinked,
				condition = this.condition.ToString(),
				action = this.action.ToString()
			});
		}

		public void FromJSON(string payload) {
			var data = JsonUtility.FromJson<GambitRow<C,A>>(payload);

			this.isEnabled = data.isEnabled;
			this.isLinked = data.isLinked;
			this.condition = data.condition;
			this.action = data.action;
		}

		// Part of the ISpawnable interface; called when a new row is populated
		public void OnSpawn(object spawner) { }

		// Part of the ISpawnable interface; called when a new row is not currently needed
		public void OnDespawn(object spawner) { }

		public abstract IGambitRow CreateDuplicate();

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

			if (this.isLinked) {
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

		public void SetCondition(Enum selection) {
			this.condition = (C)selection;
		}

		public Enum GetCondition() {
			return this.condition;
		}

		public void SetAction(Enum selection) {
			this.action = (A)selection;
		}

		public Enum GetAction() {
			return this.action;
		}

		public void SetIsLinked(bool state) {
			this.isLinked = state;
		}

		public bool GetIsLinked() {
			return this.isLinked;
		}

		public void SetIsEnabled(bool state) {
			this.isEnabled = state;
		}

		public bool GetIsEnabled() {
			return this.isEnabled;
		}
	}
}