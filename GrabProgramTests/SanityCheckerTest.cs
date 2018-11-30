using GrabProgram;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GrabProgramTests {
	[TestClass]
	public class SanityCheckerTest {
		[TestMethod]
		public void TestGood1659File() {
			SanityChecker sc = new SanityChecker(@"..\..\..\70479A1659.CNC", @"1659");
			sc.Check();
			Assert.IsTrue(sc.OK);
		}

		[TestMethod]
		public void TestBad1659File() {
			SanityChecker sc = new SanityChecker(@"..\..\..\126761B2147.CNC", @"1659");
			sc.Check();
			Assert.IsFalse(sc.OK);
		}

		[TestMethod]
		public void TestGood2147File() {
			SanityChecker sc = new SanityChecker(@"..\..\..\126754B2147.CNC", @"2147");
			sc.Check();
			Assert.IsTrue(sc.OK);
		}

		[TestMethod]
		public void TestBad2147File() {
			SanityChecker sc = new SanityChecker(@"..\..\..\70590A1659.CNC", @"2147");
			sc.Check();
			Assert.IsFalse(sc.OK);
		}

		[TestMethod]
		public void TestCouldNotParse() {
			SanityChecker sc = new SanityChecker(@"..\..\..\test.CNC", @"2147");
			sc.Check();
			Assert.IsTrue(sc.OK);
			Assert.IsFalse(sc.SuccessfullyParsed);
		}

		[TestMethod]
		public void TestEmptyFile() {
			SanityChecker sc = new SanityChecker(@"..\..\..\test2.CNC", @"2147");
			Assert.ThrowsException<System.ArgumentNullException>(new System.Action(sc.Check));
			Assert.IsFalse(sc.OK);
			Assert.IsFalse(sc.SuccessfullyParsed);
		}
	}
}
