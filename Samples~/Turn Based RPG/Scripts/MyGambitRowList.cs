using System.Collections.Generic;

using jmayberry.GambitSystem;

public class MyGambitRowList : GambitRowList<MyGambitConditions, MyGambitCombatActions>, IGambitRowList {
	public MyGambitRowList(List<MyGambitRow> rowList) {
		this.gambitRowList = new List<IGambitRow>();
		foreach (MyGambitRow row in rowList) {
			this.gambitRowList.Add(row);
		}
	}

	public MyGambitRowList(List<string> jsonRowList) {
		this.gambitRowList = new List<IGambitRow>();
		foreach (string jsonRow in jsonRowList) {
			this.gambitRowList.Add(new MyGambitRow(jsonRow));
		}
	}
	public override void AddEmpty() {
		this.gambitRowList.Add(new MyGambitRow());
	}
}
