using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitAger
{
	class bitField
	{
		enum bitFieldArg { CopyBytesAsRef };

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

			this.bytes = new byte[nbits/8 + extra];
			Bytes.CopyTo(this.bytes, 0);
		}

		private bitField(int nBits, byte[] Bytes, bitFieldArg aoeu)
		{
			this.nbits = nBits;
			this.bytes = Bytes;
		}

		public static bitField operator <<(bitField a, int n)
		{
			byte[] bs = new byte[a.bytes.Length];

			a.bytes.CopyTo(bs, 0);

			while (n-- > 0)
				for (int i = 0; i<bs.Length; i++)
				{
					bs[i] = (byte) (bs[i] << 1);

					if(i<bs.Length-1)
						 bs[i] |= (byte) ((bs[i+1] & 0x80) >> 7);
				}

			return new bitField(a.nbits, bs);
		}

		public static bitField operator |(bitField a, uint orMask)
		{
			byte[] bs = new byte[a.bytes.Length];
			int maxLen = 4;

			a.bytes.CopyTo(bs, 0);

			if (a.bytes.Length < 4)
				maxLen = a.bytes.Length;

			for (int i = 0; i < maxLen; i++)
			{
				bs[a.bytes.Length-i-1] |= (byte) ((orMask & (0xFFu<<i*8)) >> i*8);
			}

			return new bitField(a.nbits, bs);
		}

		public static explicit operator System.UInt64(bitField a)
		{
			ulong retVal = 0;
			int maxLen = 8;
			byte highMask = 0xFF;
			byte[] bs = new byte[a.bytes.Length];

			a.bytes.CopyTo(bs, 0);

			if (a.nbits % 8 != 0)
				highMask >>= (a.nbits % 8);
			bs[0] &= highMask;

			if (bs.Length < 8)
				maxLen = bs.Length;

			for (int i = 0; i < maxLen; i++)
			{
				retVal |= ((ulong)bs[maxLen - i - 1]) << i * 8;
			}

			return retVal;
		}

		public static explicit operator System.Int64(bitField a)
		{
			long retVal = -1;
			int maxLen = 8;
			byte highMask = 0xFF;
			byte[] bs = new byte[a.bytes.Length];
			int highBit;

			if (a.nbits % 8 == 0)
				highBit = a.bytes[0] >> 7;
			else
				highBit = a.bytes[0] >> ((a.nbits % 8)-1);

			if (highBit == 0)
				return (long)(ulong)a;

			a.bytes.CopyTo(bs, 0);

			if (a.nbits % 8 != 0)
				highMask <<= a.nbits%8;
			bs[0] |= highMask;

			if (bs.Length < 8)
				maxLen = bs.Length;

			for (int i = 0; i < maxLen; i++)
			{
				retVal ^= ((long)(byte)~bs[maxLen - i - 1]) << i * 8;
			}

			return retVal;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < bytes.Length; i++ )
			{
				sb.AppendFormat("{0:X2} ", bytes[i]);
			}
			return sb.ToString();

			if (nbits <= 64)
				return ((ulong)this).ToString("X");
			else
				return base.ToString();
		}

		public bitField littleEndianValue()
		{
			int shift = 0;
			int len = bytes.Length;
			bitField retVal = new bitField(nbits, bytes);

			if (nbits <= 8)
				return retVal;

			if (nbits % 8 != 0)
				shift = (8 - nbits % 8);
			retVal <<= shift;
			for (int i = 0; i < len / 2; i++)
			{
				byte tmp = retVal.bytes[i];

				retVal.bytes[i] = retVal.bytes[len - i - 1];
				retVal.bytes[len - i - 1] = tmp;
			}
			retVal.bytes[0] >>= shift;
	
			return retVal;
		}
	}
}
