using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitAger
{
	public class FieldInterpreter
	{
		private static string[] types = { "sint", "uint", "float", "fixed", "spare" };
		struct Field
		{
			public string type;
			public string name;
			public int bitwidth;
			public bool endianness; // True = Big, False = Little
		};
		private byte lastByte;
		private uint lastBits = 8;
		private System.IO.FileStream fStream;
		private List<Field> field = new List<Field>();

		public FieldInterpreter(string filename, string descriptor)
		{
			string[] desc = System.IO.File.ReadAllLines(descriptor); // TODO: Catch exception.

			this.fStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read); // TODO: Catch exception.

			foreach (string line in desc)
			{
				Field f = new Field();
				string[] tokens = line.Split(new string[] { " ", "\t" }, 4, StringSplitOptions.RemoveEmptyEntries); // TODO: Catch exception?
				if (tokens.Length != 4)
					return;
				f.type = tokens[0];
				f.name = tokens[1];
				f.bitwidth = Convert.ToInt32(tokens[2]); // TODO: Catch exception.
				if (tokens[3] == "L")
					f.endianness = false;
				if (tokens[3] == "B")
					f.endianness = true;
				field.Add(f);
			}
		}

		private object getNext(Field f)
		{
			int bytesToRead = (int) Math.Ceiling((f.bitwidth - 8 + lastBits) / 8.0);
			byte[] newBytes;
			uint locLastBits = lastBits;

			newBytes = new byte[bytesToRead+1];
			newBytes[0] = lastByte;

			fStream.Read(newBytes, 1, bytesToRead);
			if (bytesToRead > 0)
				lastByte = newBytes[newBytes.Length-1];

			lastBits = (uint)f.bitwidth + lastBits - 8 * (uint)bytesToRead;

			if (f.bitwidth <= 64)
			{
				bitField retVal = new bitField(f.bitwidth);
				int bitsUsed = 0;
				int bitCounter = 0;

				while (bitsUsed < f.bitwidth)
				{
					byte curByte = newBytes[bitCounter / 8];
					uint curBit;

					if (bitCounter < locLastBits)
					{
						bitCounter++;
						continue;
					}

					curByte <<= (bitCounter % 8);
					curBit = (curByte & (0x80u)) >> 7;

					retVal <<= 1;
					retVal |= curBit;
					bitsUsed++;
					bitCounter++;
				}
				if (!f.endianness)
				{
					retVal = retVal.littleEndianValue();
				}
				return retVal;
			}

			switch (f.type)
			{
				case "uint":
					
					break;
				case "sint":

					break;
			}
			return 0;
		}

		public void Print()
		{
			foreach (Field f in field)
			{
				Console.WriteLine("{0}: {1}", f.name, getNext(f));
			}
		}

		public bool isValid()
		{
			if (field.Count() == 0 || fStream == null)
				return false;
			foreach (Field f in field)
			{
				if (f.type == null || f.name == null || f.bitwidth == 0)
					return false;
			}
			return true;
		}
	}
}
