
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Reflection;

namespace jmayberry.GambitSystem {
	public interface IGambitContext {
	}

	public interface IGambitRowList {
		public IEnumerator<IGambitRow> GetEnumerator();

		public void AddEmpty();

		public void Add(IGambitRow row);

		public void Remove(int index);
		public void Remove(IGambitRow row);
    }

	public interface IGambitCharacter {
		public IGambitRowList GetGambitRowList();

		public void SetGambitRowList(IGambitRowList gambitRowList);

	}

	public abstract class GambitRowList<C, A> : IGambitRowList where C : Enum where A : Enum {
		public List<IGambitRow> gambitRowList;

		public IEnumerator<IGambitRow> GetEnumerator() {
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

		public void Paste(IGambitRow newRow, int index) {
			this.gambitRowList[index] = newRow;
		}

		public void Duplicate(int index) {
			this.gambitRowList.Insert(index, this.gambitRowList[index].CreateDuplicate());
        }

        public void Remove(IGambitRow row) {
            this.gambitRowList.Remove(row);
        }

        public void Remove(int index) {
            this.gambitRowList.RemoveAt(index);
        }

        public void Clear(int index) {
			this.gambitRowList[index].Clear();
		}

		public abstract void AddEmpty();

		public void Add(IGambitRow row) {
			this.gambitRowList.Add(row);
		}
	}
}