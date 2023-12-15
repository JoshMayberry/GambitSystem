
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine;

namespace jmayberry.GambitSystem {
    public interface IGambitRowList<T, C, A> where T : IGambitRow<C, A> where C : Enum where A : Enum {
        List<T> gambitRowList { get; set; }

        T CreateEmptyRow();

		bool EvaluateGambits(IGambitContext context);

		void Paste(T newRow, int index);

		void Duplicate(int index);

		void Remove(T row);

		void Remove(int index);

		void Clear(int index);

		void AddEmpty();

		void Add(T row);

    }

	[System.Serializable]
    public abstract class GambitRowList<T, C, A> : ScriptableObject, IGambitRowList<T, C, A> where T : IGambitRow<C, A> where C : Enum where A : Enum {
        public abstract List<T> gambitRowList { get; set; }

        public abstract T CreateEmptyRow();

        public IEnumerator<T> GetEnumerator() {
			foreach (var row in gambitRowList) {
				yield return row;
			}
		}

        public virtual bool EvaluateGambits(IGambitContext context) {
			bool previousWasLinked = false;
			bool previousLinkPassed = false;
			foreach (var gambitRow in this) {
				if (previousWasLinked && !previousLinkPassed) {
					previousWasLinked = !gambitRow.isLinked; // Account for multi-line links
					continue; // Skip this one, because the previously linked row failed it's condition
				}

				if (gambitRow.isLinked) {
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

        public virtual void Paste(T newRow, int index) {
			this.gambitRowList[index] = newRow;
		}

		public virtual void Duplicate(int index) {
			//this.gambitRowList.Insert(index, this.gambitRowList[index].CreateDuplicate());
        }

        public virtual void Remove(T row) {
            this.gambitRowList.Remove(row);
        }

        public virtual void Remove(int index) {
            this.gambitRowList.RemoveAt(index);
        }

        public virtual void Clear(int index) {
			this.gambitRowList[index].Clear();
		}

        public virtual void AddEmpty() {
            this.gambitRowList.Add(this.CreateEmptyRow());
        }

        public virtual void Add(T row) {
			this.gambitRowList.Add(row);
		}
	}
}