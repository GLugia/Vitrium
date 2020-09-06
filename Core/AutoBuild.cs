using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace Vitrium.Core
{
	internal class AutoBuild
	{
		private static List<string> lines;

		internal static void Load()
		{
			lines = new List<string>();

			using (var fs = File.OpenRead(Path.Combine(Path.Combine(Program.SavePath, "Mod Sources"), Path.Combine("Vitrium", "build.txt"))))
			{
				using (var reader = new StreamReader(fs))
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
			if (lines == null || !lines.Any())
			{
				return;
			}

			var sources = Path.Combine(Program.SavePath, "Mod Sources");
			var vitrium = Path.Combine(sources, "Vitrium");
			var build = Path.Combine(vitrium, "build.txt");

			using (var fs = File.Open(build, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				using (var writer = new StreamWriter(fs))
				{
					foreach (var line in lines)
					{
						if (line.Contains("version"))
						{
							var index = line.IndexOf('=');
							var value = line.Substring(index + 1).Trim();
							var version = Version.Parse(value);
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
