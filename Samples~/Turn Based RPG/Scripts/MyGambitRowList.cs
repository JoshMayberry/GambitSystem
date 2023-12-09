using System.Collections.Generic;
using UnityEngine;


using jmayberry.GambitSystem;

[CreateAssetMenu(fileName = "GambitRowList", menuName = "Testing/GambitRow", order = 1)]
public class MyGambitRowList : GambitRowList {
    [SerializeField] public new List<MyGambitRow> gambitRowList;

    public MyGambitRowList(List<MyGambitRow> gambitRowList) {
        this.gambitRowList = gambitRowList;
    }

    public override GambitRow CreateEmptyRow() {
        return new MyGambitRow();
    }
}
