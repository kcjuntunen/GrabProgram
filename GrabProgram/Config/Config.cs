using System;

namespace GrabProgram.Config {
	[Serializable]
	public class Config {
		public Machines MachineList { get; set; }
		public StorageName Storage { get; set; }
	}
}
