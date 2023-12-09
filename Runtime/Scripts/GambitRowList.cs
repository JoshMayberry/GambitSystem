
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine;

namespace jmayberry.GambitSystem {
	public interface IGambitContext {
	}

	public interface IGambitCharacter {
		public GambitRowList GetGambitRowList();

		public void SetGambitRowList(GambitRowList gambitRowList);
	}

	public abstract class GambitRowList : ScriptableObject {
        public List<GambitRow> gambitRowList;

		public abstract GambitRow CreateEmptyRow();

        public IEnumerator<GambitRow> GetEnumerator() {
			foreach (var row in gambitRowList) {
				yield return row;
			}
		}

        public bool EvaluateGambits(IGambitContext context) {
			bool previousWasLinked = false;
			bool previousLinkPassed = false;
			foreach (var gambitRow in this.gambitRowList) {
				if (previousWasLinked && !previousLinkPassed) {
					previousWasLinked = !gambitRow.IsLinked(); // Account for multi-line links
					continue; // Skip this one, because the previously linked row failed it's condition
				}

				if (gambitRow.IsLinked()) {
					previousLinkPassed = gambitRow.EvaluateGambit(context);
					previousWasLinked = true;
					continue;
				}

				if (gambitRow.EvaluateGambit(context)) {
					return true;
				}
			}
			return false;
		}

		public void Paste(GambitRow newRow, int index) {
			this.gambitRowList[index] = newRow;
		}

		public void Duplicate(int index) {
			this.gambitRowList.Insert(index, this.gambitRowList[index].CreateDuplicate());
        }

        public void Remove(GambitRow row) {
            this.gambitRowList.Remove(row);
        }

        public void Remove(int index) {
            this.gambitRowList.RemoveAt(index);
        }

        public void Clear(int index) {
			this.gambitRowList[index].Clear();
		}

        public void AddEmpty() {
            this.gambitRowList.Add(this.CreateEmptyRow());
        }

        public void Add(GambitRow row) {
			this.gambitRowList.Add(row);
		}
	}
}