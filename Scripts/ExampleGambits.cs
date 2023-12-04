using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using jmayberry.GambitSystem;
using UnityEngine.UI;

public class AIContext : IGambitContext {
    public AICharacter Character { get; private set; }
    public List<AICharacter> Enemies { get; private set; }
    public AICharacter Target { get; set; }

    // Constructor
    public AIContext(AICharacter character, List<AICharacter> enemies) {
        Character = character;
        Enemies = enemies;
    }
}

public class AICharacter {
    public float Health { get; private set; }
    public Vector3 Position { get; private set; }

    public void Attack(AICharacter target) {
        // Attack logic here
    }

    public void Heal(float amount) {
        Health += amount;
    }
}

public enum MyGambitConditions {
    HealthBelow,
    EnemyInRange,
}

public enum MyGambitCombatActions {
    Attack,
    Heal,
}

public class MyGambitRow : GambitRow<MyGambitConditions, MyGambitCombatActions> {
    public MyGambitRow() : base() { }

    public MyGambitRow(GambitRow<MyGambitConditions, MyGambitCombatActions> duplicateFrom) : base(duplicateFrom) { }

    public MyGambitRow(MyGambitConditions condition, MyGambitCombatActions action) : base(condition, action) { }

    public MyGambitRow(MyGambitConditions condition, MyGambitCombatActions action, bool isEnabled, bool isLinked) : base(condition, action, isEnabled, isLinked) { }

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

    public override GambitRow<MyGambitConditions, MyGambitCombatActions> CreateDuplicate() {
        return new MyGambitRow(this);
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

public class MyGambitManager : GambitManagerBase<MyGambitConditions, MyGambitCombatActions> {

}

public class ExampleCharacter {
    public List<MyGambitRow> gambitRows;

    public ExampleCharacter() {
        this.gambitRows = new List<MyGambitRow> {
            new MyGambitRow(){isEnabled=true, isLinked=false, condition=MyGambitConditions.EnemyInRange, action=MyGambitCombatActions.Attack},
            new MyGambitRow(){isEnabled=true, isLinked=false, condition=MyGambitConditions.HealthBelow, action=MyGambitCombatActions.Heal},
        };
    }

    public void TakeTurn(IGambitContext context) {
        MyGambitManager.instance.EvaluateGambits(context, this.gambitRows);
    }
}


