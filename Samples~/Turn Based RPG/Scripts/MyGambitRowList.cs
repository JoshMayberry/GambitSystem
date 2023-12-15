using System.Collections.Generic;
using UnityEngine;


using jmayberry.GambitSystem;

[System.Serializable]
[CreateAssetMenu(fileName = "GambitRowList", menuName = "Testing/GambitRow", order = 1)]
public class MyGambitRowList : GambitRowList<MyGambitRow, MyGambitConditions, MyGambitCombatActions>, IGambitRowList<MyGambitRow, MyGambitConditions, MyGambitCombatActions> {
    public override List<MyGambitRow> gambitRowList { get => this._gambitRowList; set => this._gambitRowList = value; }
    [SerializeField] private List<MyGambitRow> _gambitRowList;

    public MyGambitRowList(List<MyGambitRow> gambitRowList) {
        this.gambitRowList = gambitRowList;
    }


    public override MyGambitRow CreateEmptyRow() {
        return new MyGambitRow();
    }
}
