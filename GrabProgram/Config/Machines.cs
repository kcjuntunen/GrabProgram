using System;
using System.Collections;
using System.Collections.Generic;

namespace GrabProgram {
	[Serializable]
	public class Machines : IList<Machine> {
		public List<Machine> _innerArray = new List<Machine>();
		
		public Machines() {

		}

		public int IndexOf(Machine item) {
			return ((IList<Machine>)_innerArray).IndexOf(item);
		}

		public void Insert(int index, Machine item) {
			((IList<Machine>)_innerArray).Insert(index, item);
		}

		public void RemoveAt(int index) {
			((IList<Machine>)_innerArray).RemoveAt(index);
		}

		public void Clear() {
			((IList<Machine>)_innerArray).Clear();
		}

		public bool Contains(Machine item) {
			return ((IList<Machine>)_innerArray).Contains(item);
		}

		public void CopyTo(Machine[] array, int arrayIndex) {
			((IList<Machine>)_innerArray).CopyTo(array, arrayIndex);
		}

		public bool Remove(Machine item) {
			return ((IList<Machine>)_innerArray).Remove(item);
		}

		public IEnumerator<Machine> GetEnumerator() {
			return ((IList<Machine>)_innerArray).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IList<Machine>)_innerArray).GetEnumerator();
		}

		public void Add(Machine item) {
			((IList<Machine>)_innerArray).Add(item);
		}

		public int Count => ((IList<Machine>)_innerArray).Count;

		public bool IsReadOnly => ((IList<Machine>)_innerArray).IsReadOnly;

		public Machine this[int index] { get => ((IList<Machine>)_innerArray)[index]; set => ((IList<Machine>)_innerArray)[index] = value; }
	}
}
