using UnityEngine;

using jmayberry.GambitSystem;
using System;

public enum MyGambitConditions {
	HealthBelow,
	EnemyInRange,
}

public enum MyGambitCombatActions {
	Attack,
	Heal,
}

[System.Serializable]
public class MyGambitRow : GambitRow {
    public new MyGambitConditions condition;
    public new MyGambitCombatActions action;

    public MyGambitRow(Enum condition, Enum action, bool isEnabled, bool isLinked) : base(condition, action, isEnabled, isLinked) {}

    public MyGambitRow() : base() {}

    public override GambitRow CreateDuplicate() {
		return new MyGambitRow(this.condition, this.action, this.isEnabled, this.isLinked);
	}

	public override bool CheckCondition(IGambitContext context) {
		if (context is not AIContext aiContext) {
			Debug.LogError("Unknown context format");
			return false;
		}

		switch (this.condition) {
			case MyGambitConditions.HealthBelow:
				return aiContext.Character.Health < 30;

			case MyGambitConditions.EnemyInRange:
				return (aiContext.Target != null) && (Vector3.Distance(aiContext.Character.Position, aiContext.Target.Position) <= 30);
		}

		Debug.LogError($"Unknown gambit condition {this.condition}");
		return false;
	}

	public override void Execute(IGambitContext context) {
		if (context is not AIContext aiContext) {
			Debug.LogError("Unknown context format");
			return;
		}

		switch (this.action) {
			case MyGambitCombatActions.Attack:
				aiContext.Character.Attack(aiContext.Target);
				return;

			case MyGambitCombatActions.Heal:
				aiContext.Character.Heal(10);
				return;
		}

		Debug.LogError($"Unknown gambit action {this.action}");
	}
}