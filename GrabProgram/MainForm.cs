using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GrabProgram {
	public partial class MainForm : Form {
		private bool initialated = false;
		private SanityChecker sc;

		public MainForm() {
			InitializeComponent();
			Location = Properties.Settings.Default.Location;
			Size = Properties.Settings.Default.Size;

			ConfigLocation = string.Format(@"{0}\{1}",
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				Properties.Settings.Default.ConfigFile);
		}

		private	DriveInfo GetDrive() {
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo drive in drives) {
				if (!drive.IsReady) {
					continue;
				}
				if (drive.VolumeLabel == Config.Storage.Name) {
					return drive;
				}
			}
			throw new Exception(
				string.Format("Couldn't find a drive with the label `{0}'. Did you insert one?\n\n", Config.Storage.Name));
		}

		private void WriteInitialConfig() {
			using (FileStream fs = new FileStream(ConfigLocation, FileMode.OpenOrCreate)) {
				try {
					XmlSerializer serializer = new XmlSerializer(Config.GetType());
					serializer.Serialize(fs, Config);
				} catch (FileNotFoundException fnfe) {
					if (!fnfe.Message.Contains(@"GrabProgram.XmlSerializers")) {
						ProcessError(fnfe, false);
					}
				} catch (Exception e) {
					ProcessError(e, false);
				}
			}
		}

		private void DoCopy() {
			FileInfo f = new FileInfo(CopySource);
			toolStripStatusLabel1.Text = string.Format(@"Copying `{0}'...", f.Name);
			try {
				File.Copy(CopySource, CopyDestination, true);
			} catch (UnauthorizedAccessException uae) {
				ProcessError(uae, false);
			} catch (ArgumentNullException an) {
				ProcessError(an, false);
			} catch (ArgumentException a) {
				ProcessError(a, false);
			} catch (PathTooLongException ptl) {
				ProcessError(ptl, false);
			} catch (DirectoryNotFoundException dnf) {
				ProcessError(dnf, false);
			} catch (FileNotFoundException fnf) {
				ProcessError(fnf, false);
			} catch (IOException io) {
				ProcessError(io, false);
			} catch (NotSupportedException ns) {
				ProcessError(ns, false);
			} catch (Exception e) {
				ProcessError(e, false);
			}
			toolStripStatusLabel1.Text = string.Format(@"Copied `{0}'.", f.Name);
		}
		/// <summary>
		/// Dump an error into the db.
		/// </summary>
		/// <param name="e">An <see cref="Exception"/> object.</param>
		public static void ProcessError(Exception e, bool DoNotInsert) {
			string[] stack = e.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			string offender = stack[stack.Length - 1].Trim();
			int len = offender.Length > 255 ? 255 : offender.Length;
			string msg_ = string.Format(@"Error `{0}' occurred {1}.", e.Message, offender);

			if (DoNotInsert) {
				MessageBox.Show(msg_, @"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			using (ErrLogTableAdapters.QueriesTableAdapter q_ =
				new ErrLogTableAdapters.QueriesTableAdapter()) {
				int uid_ = 0;
				uid_ = Convert.ToInt32(q_.UserQuery(Environment.UserName));
				int aff = q_.InsertError(
					DateTime.Now,
					uid_,
					e.HResult,
					e.Message,
					offender.Substring(0, len),
					false,
					@"COPY PROGRAM");
				if (aff > 0) {
					msg_ += "\n" + @"This error has been reported.";
					MessageBox.Show(msg_, @"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				} else {
					msg_ += "\n" + @"This error failed to be reported.";
					MessageBox.Show(msg_, @"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}

		private void button1_Click(object sender, EventArgs e) {
			if (sc.OK) {
				DoCopy();
			} else {
				MessageBox.Show(string.Format("This program appears to be made for `{0}'.\n\n" +
					"First line says:\n{1}", sc.MachNumFromFile, sc.Line));
			}
		}

		private void Form1_Load(object sender, EventArgs e) {
			if (!new FileInfo(ConfigLocation).Exists) {
				WriteInitialConfig();
			}

			using (FileStream fs = new FileStream(ConfigLocation, FileMode.Open)) {
				try {
					XmlSerializer serializer = new XmlSerializer(Config.GetType());
					Config = (Config.Config)serializer.Deserialize(fs);

					comboBox1.DataSource = Config.MachineList._innerArray;
					comboBox1.ValueMember = @"MachNum";
					comboBox1.DisplayMember = @"Name";
				} catch (FileNotFoundException fnfe) {
					if (!fnfe.Message.Contains(@"GrabProgram.XmlSerializers")) {
						ProcessError(fnfe, false);
					}
				} catch (Exception ex) {
					ProcessError(ex, false);
				}
			}
			initialated = true;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if (!initialated) {
				return;
			}
			toolStripStatusLabel1.Text = @"-";
			ComboBox cb = (sender as ComboBox);
			if (cb.SelectedItem == null) {
				return;
			}
			if (!(cb.SelectedItem is Machine v_)) {
				return;
			}

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = v_.Path;
			ofd.Filter = @"Machine Programs (*.CNC)|*.CNC";
			DialogResult dr = ofd.ShowDialog(this);

			if (dr == DialogResult.Cancel) {
				return;
			}

			CopySource = ofd.FileName;
			FileInfo f = new FileInfo(CopySource);
			try {
				CopyDestination = string.Format(@"{0}PR{1}.CNC", GetDrive().Name, v_.MachNum);
				MachNum = v_.MachNum;
				label1.Text = string.Format(@"{0} → {1}", f.Name, CopyDestination);
				toolStripStatusLabel1.Text = string.Format(@"Selected `{0}'.", f.Name);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			sc = new SanityChecker(CopySource, MachNum);
			try {
				sc.Check();
				label2.Text = sc.Line;
				if (!sc.SuccessfullyParsed) {
					MessageBox.Show(@"Couldn't verify that the program matches the machine.",
						@"Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
				}
			} catch (ArgumentNullException) {
				MessageBox.Show(@"CNC file is empty.",
					@"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			} catch (Exception ex) {
				ProcessError(ex, false);
			}
		}

		public string CopySource { get; set; }
		public string CopyDestination { get; set; }
		public string ConfigLocation { get; set; }
		public string MachNum { get; set; }

		public Config.Config Config { get; set; } = new Config.Config {
			MachineList = new Machines {
					new Machine {
					Name = @"GIO 1659",
					MachNum = @"1659",
					Path = @"S:\shared\general\CNC PROGRAMS\CMS-1659\"
					},
					new Machine {
					Name = @"GIO 2147",
					MachNum = @"2147",
					Path = @"S:\shared\general\CNC PROGRAMS\CMS-2147\"
					}
				},
			Storage = new StorageName {
				Name = @"CMS-GIOTTO"
			}
		};

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			Properties.Settings.Default.Location = Location;
			Properties.Settings.Default.Size = Size;
			Properties.Settings.Default.Save();
		}
	}
}
