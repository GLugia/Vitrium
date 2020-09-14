using System;
using System.Collections.Generic;
using System.IO;
using Terraria;

namespace Vitrium.Core
{
	internal class AutoBuild
	{
		private static List<string> lines;

		internal static void Load()
		{
			lines = new List<string>();

			using (FileStream fs = File.OpenRead(Path.Combine(Path.Combine(Program.SavePath, "Mod Sources"), Path.Combine("Vitrium", "build.txt"))))
			{
				using (StreamReader reader = new StreamReader(fs))
				{
					while (!reader.EndOfStream)
					{
						lines.Add(reader.ReadLine());
					}
				}
			}
		}

		internal static void Save(bool updatebuild = false, bool updaterevision = true)
		{
			if (lines == null || lines.Count <= 0)
			{
				return;
			}

			string sources = Path.Combine(Program.SavePath, "Mod Sources");
			string vitrium = Path.Combine(sources, "Vitrium");
			string build = Path.Combine(vitrium, "build.txt");

			using (FileStream fs = File.Open(build, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				using (StreamWriter writer = new StreamWriter(fs))
				{
					foreach (string line in lines)
					{
						if (line.Contains("version"))
						{
							int index = line.IndexOf('=');
							string value = line.Substring(index + 1).Trim();
							Version version = Version.Parse(value);
							writer.WriteLine($"version = {new Version(version.Major, version.Minor, updatebuild ? version.Build + 1 : version.Build, updaterevision ? version.Revision + 1 : version.Revision)}");
						}
						else
						{
							writer.WriteLine(line);
						}
					}
				}
			}
		}
	}
}
