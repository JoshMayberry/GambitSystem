using UnityEngine;
using System;

using jmayberry.Spawner;


namespace jmayberry.GambitSystem {
	public interface IGambitRow<C, A> where C : Enum where A : Enum {
        bool isEnabled { get; set; }

        bool isLinked {get; set;}

        C condition {get; set;}

        A action {get; set;}

        IGambitRow<C, A> CreateDuplicate();

		bool CheckCondition(IGambitContext context);

		void Execute(IGambitContext context);

		bool EvaluateGambit(IGambitContext context);

        string ToJSON();

		void FromJSON(string payload);

		void Clear();
    }

    [System.Serializable]
	public abstract class GambitRow<C, A> : ISpawnable, IGambitRow<C, A> where C : Enum where A : Enum {
        public bool isEnabled { get => this._isEnabled; set => this._isEnabled = value; }
        [SerializeField] private bool _isEnabled;

        public bool isLinked { get => this._isLinked; set => this._isLinked = value; } // Used to create multi-condition logic
        [SerializeField] private bool _isLinked;

        public abstract C condition { get; set; }

		public abstract A action { get; set; }

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

		public virtual string ToJSON() {
			return JsonUtility.ToJson(new {
				isEnabled = this.isEnabled,
				isLinked = this.isLinked,
				condition = this.condition.ToString(),
				action = this.action.ToString()
			});
		}

        public virtual void FromJSON(string payload) {
			var data = JsonUtility.FromJson<GambitRow<C,A>>(payload);

			this.isEnabled = data.isEnabled;
			this.isLinked = data.isLinked;
			this.condition = data.condition;
			this.action = data.action;
		}

        // Part of the ISpawnable interface; called when a new row is populated
        public virtual void OnSpawn(object spawner) { }

        // Part of the ISpawnable interface; called when a new row is not currently needed
        public virtual void OnDespawn(object spawner) { }

		public abstract IGambitRow<C, A> CreateDuplicate();

		// When implemented, this should have a switch case that checks what to do based on which enum was selected by the conditionSelector
		public abstract bool CheckCondition(IGambitContext context);

		// When implemented, this should have a switch case that does an action based on which enum was selected by the actionSelector
		public abstract void Execute(IGambitContext context);

        public virtual bool EvaluateGambit(IGambitContext context) {
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

        public virtual void Clear() {
			this.isEnabled = false;
			this.isLinked = false;
			this.condition = (C)(object)(0);
			this.action = (A)(object)(0);
		}
    }
}