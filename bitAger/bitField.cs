using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitAger
{
	class bitField
	{
		byte[] bytes;
		int nbits;

		public bitField(int nBits)
		{
			int extra = 0;

			this.nbits = nBits;

			if (nBits % 8 != 0)
				extra = 1;

			bytes = new byte[nbits/8 + extra];
		}

		public bitField(int nBits, byte[] Bytes)
		{
			int extra = 0;

			this.nbits = nBits;

			if (nBits % 8 != 0)
				extra = 1;

			bytes = new byte[nbits/8 + extra];
		}

		public static bitField operator <<(bitField a, int n)
		{
			byte[] bs = new byte[a.bytes.Length];

			for (int i = 0; i<bs.Length; i++)
			{
				bs[i] = (byte) ((a.bytes[i] << 1) | (a.bytes[i+1] & 0x01));
			}
			return new bitField(n, bs);
		}

		public byte[] littleEndianValue()
		{
			return null;
		}
	}
}
