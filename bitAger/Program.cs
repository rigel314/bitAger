using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bitAger
{
	static class Program
	{
		public static bool windowed = true;
		public static List<string> inputFiles = new List<string>();
		public static List<string> descriptorFiles = new List<string>();

		private static List<FieldInterpreter> interpreters = new List<FieldInterpreter>();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var parser = new ArgParser(args);
			
			parser.Parse();

			if(inputFiles.Count() != descriptorFiles.Count() && descriptorFiles.Count > 1)
			{
				Console.WriteLine("Mismatched number of input and descriptor files.");
				return;
			}
			if (descriptorFiles.Count == 1)
			{
				foreach (string name in inputFiles)
					descriptorFiles.Add(descriptorFiles[0]);
			}

			foreach (string name in inputFiles)
				Console.WriteLine("{0}", name);

			for (int i = 0; i < inputFiles.Count; i++)
			{
				var fi = new FieldInterpreter(inputFiles[i], descriptorFiles[i]);
				if (fi.isValid())
				{
					interpreters.Add(fi);
				}
				else
				{
					Console.WriteLine("Error in input descriptor: {0}", descriptorFiles[i]);
					return;
				}
			}

			if (windowed)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
			else
			{
				foreach (FieldInterpreter fi in interpreters)
				{
					fi.Print();
				}
			}
		}
	}
	class ArgParser
	{
		private string[] args;

		public ArgParser(string[] argsIn)
		{
			args = (string[]) argsIn.Clone();
		}
		public void Parse()
		{
			bool doubleDashFlag = false;
			for (int i = 0; i < args.Length; i++)
			{
				//Console.WriteLine("Arg[{0}] = [{1}]", i, args[i]);
				if (args[i] == "--" && !doubleDashFlag)
				{
					doubleDashFlag = true;
					continue;
				}
				if (args[i] == "-d" && i < args.Length - 1 && !doubleDashFlag)
				{
					Program.descriptorFiles.Add(args[i + 1]);
					i++;
					continue;
				}
				if (args[i] == "-n" && !doubleDashFlag)
					Program.windowed = false;
				else
					Program.inputFiles.Add(args[i]);
			}
		}
	}
}
