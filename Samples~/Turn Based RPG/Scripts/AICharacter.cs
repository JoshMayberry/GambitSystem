using UnityEngine;

using jmayberry.GambitSystem;

public class AICharacter : MonoBehaviour, IGambitCharacter {
	public float Health { get; private set; }
	public Vector3 Position { get; private set; }

	public void Attack(AICharacter target) {
		// Attack logic here
	}

	public void Heal(float amount) {
		Health += amount;
	}

	public MyGambitRowList myGambitRowList;

	public void TakeTurn(IGambitContext context) {
		this.myGambitRowList.EvaluateGambits(context);
	}

	public IGambitRowList GetGambitRowList() {
		return myGambitRowList;
	}

	public void SetGambitRowList(IGambitRowList gambitRowList) {
		this.myGambitRowList.gambitRowList.Clear();

		foreach (IGambitRow row in gambitRowList) {
			this.myGambitRowList.gambitRowList.Add(row);
		}
	}
}
