using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NUnitGuineaPig
{
	class TestClass
	{
		[TestCaseSource(nameof(FilesInCurrentDirectory))]
		public void FileExists(string name)
		{
			var fullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, name);
			FileAssert.Exists(fullPath);
		}

		public static IEnumerable FilesInCurrentDirectory
			=> new DirectoryInfo(TestContext.CurrentContext.TestDirectory)
			.GetFiles()
			.Select(f => f.Name);
	}
}
