using System.IO;
using System.Text.RegularExpressions;

namespace GrabProgram {
	public class SanityChecker {
		public string RxString { get; set; } = @" ([0-9]{4}) ";
		public Regex Rx { get; set; }
		public FileInfo Info { get; set; }
		public string MachNum { get; set; } = string.Empty;
		public string MachNumFromFile { get; set; }  = string.Empty;
		public bool OK { get; set; } = false;
		public string Line { get; set; } = string.Empty;
		public bool SuccessfullyParsed { get; set; } = false;

		public SanityChecker(string file, string machnum) {
			Rx = new Regex(RxString);
			Info = new FileInfo(file);
			MachNum = machnum;
		}

		public void Check() {
			try {
				using (FileStream fs = Info.OpenRead()) {
					try {
						using (StreamReader sr = new StreamReader(fs)) {
							Line = sr.ReadLine();
						}
					} catch (System.OutOfMemoryException) {
						throw;
					} catch (System.ArgumentNullException) {
						throw;
					} catch (System.ArgumentException) {
						throw;
					}
				}
			} catch (DirectoryNotFoundException) {
				throw;
			} catch (IOException) {
				throw;
			} catch (System.UnauthorizedAccessException) {
				throw;
			}

			try {
				Match match = Rx.Match(Line);
				MachNumFromFile = match.Groups[1].Value;
				SuccessfullyParsed = match.Groups.Count == 2;
				OK = (match.Groups[1].Value == MachNum) || !SuccessfullyParsed;
			} catch (System.ArgumentNullException) {
				throw;
			} catch (RegexMatchTimeoutException) {
				throw;
			}

		}

	}
}
