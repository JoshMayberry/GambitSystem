using UnityEngine;

using jmayberry.GambitSystem;

public enum MyGambitConditions {
	HealthBelow,
	EnemyInRange,
}

public enum MyGambitCombatActions {
	Attack,
	Heal,
}

public class MyGambitRow : GambitRow<MyGambitConditions, MyGambitCombatActions>, IGambitRow {
	public MyGambitRow() : base() { }

	public MyGambitRow(string jsonPayload) : base(jsonPayload) { }

	public MyGambitRow(MyGambitConditions condition, MyGambitCombatActions action, bool isEnabled, bool isLinked) : base(condition, action, isEnabled, isLinked) { }

	public override IGambitRow CreateDuplicate() {
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