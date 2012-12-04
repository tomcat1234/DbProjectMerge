using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DbProjectMerge
{
	class Program
	{
		private static string namespaceName = "http://schemas.microsoft.com/developer/msbuild/2003";
		// Project > ItemGroup > Folder [Include]
		// Project > ItemGroup > NotInBuild [Include]
		// Project > ItemGroup > Build [Include]

		static void Main(string[] args)
		{
			switch (args.Length)
			{
				case 1:
					Sort(args[0]);
					break;
				case 2:
					Merge(args[0], args[1]);
					break;
				default:
					PrintUsage();
					break;
			}

		}
		private static void PrintUsage()
		{
			string str = @"
This program sort or merge a list of the folders and files in Visual Studio Family DB Project File (.dbproj).

Sort:
	{0} myproject.dbproj

Merge:
	{0} old.dbproj new.dbproj
	(Merged project file is based on 'new.dbproj')
";

			Console.Write(string.Format(str, Path.GetFileName(Environment.GetCommandLineArgs()[0])));
		}

		private static void Sort(string filePathXml)
		{
			XElement xelm = XElement.Load(filePathXml);

			// Folderのマージ
			SortFolder(xelm);

			// SQLファイルのマージ
			SortFile(xelm);

			string filePathXmlSorted = filePathXml + ".sort";
			xelm.Save(filePathXmlSorted);
		}
		private static void Merge(string filePathXmlOld, string filePathXmlNew)
		{
			XElement xelmNew = XElement.Load(filePathXmlNew);
			XElement xelmOld = XElement.Load(filePathXmlOld);

			// Folderのマージ
			MergeFolder(xelmNew, xelmOld);

			// SQLファイルのマージ
			MergeFile(xelmNew, xelmOld);

			string filePathXmlMerged = filePathXmlNew + ".merge";
			xelmNew.Save(filePathXmlMerged);
		}

		private static void SortFolder(XElement xelm)
		{
			XNamespace ns = namespaceName;
			var list = xelm.Descendants(ns + "Folder");

			var listSorted = list
				.Distinct(e => e.Attribute("Include").Value)
				.OrderBy(e => e.Attribute("Include").Value)
				.Select(e => new XElement(ns + "Folder", new XAttribute("Include", e.Attribute("Include").Value)))
				.ToArray();


			list.Select(e => e.Parent).Distinct().Remove();

			xelm.Add(new XElement(ns + "ItemGroup", listSorted));
		}
		private static void SortFile(XElement xelm)
		{
			XNamespace ns = namespaceName;
			var listNew = xelm.Descendants(ns + "NotInBuild")
				.Union(
					xelm.Descendants(ns + "Build"),
					e => e.Attribute("Include").Value
				);

			var listSorted = listNew
				.Distinct(e => e.Attribute("Include").Value)
				.OrderBy(e => e.Attribute("Include").Value)
				.Select(e => new XElement(ns + "NotInBuild", new XAttribute("Include", e.Attribute("Include").Value)))
				.ToArray();


			listNew.Select(e => e.Parent).Distinct().Remove();

			xelm.Add(new XElement(ns + "ItemGroup", listSorted));
		}
		private static void MergeFolder(XElement xelmNew, XElement xelmOld)
		{
			XNamespace ns = namespaceName;
			var listNew = xelmNew.Descendants(ns + "Folder");
			var listOld = xelmOld.Descendants(ns + "Folder");

			var listMerged = listNew
				.Union(listOld, e => e.Attribute("Include").Value)
				.OrderBy(e => e.Attribute("Include").Value)
				.Select(e => new XElement(ns + "Folder", new XAttribute("Include", e.Attribute("Include").Value)))
				.ToArray();


			listNew.Select(e => e.Parent).Distinct().Remove();

			xelmNew.Add(new XElement(ns + "ItemGroup", listMerged));
		}
		private static void MergeFile(XElement xelmNew, XElement xelmOld)
		{
			XNamespace ns = namespaceName;
			var listNew = xelmNew.Descendants(ns + "NotInBuild")
				.Union(
					xelmNew.Descendants(ns + "Build"),
					e => e.Attribute("Include").Value
				);
			var listOld = xelmOld.Descendants(ns + "NotInBuild")
				.Union(
					xelmOld.Descendants(ns + "Build"),
					e => e.Attribute("Include").Value
				);

			var listMerged = listNew
				.Union(listOld, e => e.Attribute("Include").Value)
				.OrderBy(e => e.Attribute("Include").Value)
				.Select(e => new XElement(ns + "NotInBuild", new XAttribute("Include", e.Attribute("Include").Value)))
				.ToArray();


			listNew.Select(e => e.Parent).Distinct().Remove();

			xelmNew.Add(new XElement(ns + "ItemGroup", listMerged));
		}
	}
}
