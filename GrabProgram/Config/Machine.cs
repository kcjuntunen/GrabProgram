using System;
using System.Runtime.Serialization;

namespace GrabProgram {
	[Serializable]
	public class Machine : ISerializable {
		public string Name { get; set; }
		public string Path { get; set; }
		public string MachNum { get; set; }

		public Machine() {

		}

		public Machine(SerializationInfo info, StreamingContext context) {
			if (info == null)
				throw new ArgumentNullException("info");
			Name = (string)info.GetValue("Name", typeof(string));
			Path = (string)info.GetValue("Path", typeof(string));
			MachNum = (string)info.GetValue("MachNum", typeof(string));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue(@"Name", Name);
			info.AddValue(@"Path", Path);
			info.AddValue(@"MachNum", MachNum);
		}

	}
}
