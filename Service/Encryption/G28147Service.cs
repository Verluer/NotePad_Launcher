using Domain.Attributes;
using Domain.IService.IEncryption;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Net.Mime.MediaTypeNames;

namespace Service.Encryption
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IG28147Service))]
    public class G28147Service : IG28147Service
    {
        static readonly byte[,] S = new byte[8, 16]
        {
        {4,10,9,2,13,8,0,14,6,11,1,12,7,15,5,3},
        {14,11,4,12,6,13,15,10,2,3,8,1,0,7,5,9},
        {5,8,1,13,10,3,4,2,14,15,12,7,6,0,9,11},
        {7,13,10,1,0,8,9,15,14,4,6,12,11,2,5,3},
        {6,12,7,1,5,15,13,8,4,10,9,14,0,3,11,2},
        {4,11,10,0,7,2,1,13,3,6,8,5,9,12,15,14},
        {13,11,4,1,3,15,5,9,0,10,14,7,6,8,2,12},
        {1,15,13,0,5,10,3,4,9,2,14,7,12,11,8,6}
        };

        static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) throw new ArgumentException("Hex length must be even");
            var b = new byte[hex.Length / 2];
            for (int i = 0; i < b.Length; i++) b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return b;
        }
        static string BytesToHex(byte[] data) => BitConverter.ToString(data).Replace("-", "");
        static uint Rotl32(uint x, int n) => (x << n) | (x >> (32 - n));
        static uint BytesToUInt32BE(byte[] b, int off) => (uint)((b[off] << 24) | (b[off + 1] << 16) | (b[off + 2] << 8) | b[off + 3]);
        static void UInt32ToBytesBE(uint v, byte[] b, int off)
        {
            b[off] = (byte)(v >> 24);
            b[off + 1] = (byte)(v >> 16);
            b[off + 2] = (byte)(v >> 8);
            b[off + 3] = (byte)v;
        }
        static uint[] MakeRoundKeys(uint[] K)
        {
            if (K.Length != 8) throw new ArgumentException("K must be length 8");
            uint[] rk = new uint[32];
            int idx = 0;
            for (int r = 0; r < 3; r++)
                for (int i = 0; i < 8; i++) rk[idx++] = K[i];
            for (int i = 7; i >= 0; i--) rk[idx++] = K[i];
            return rk;
        }
        static uint F_with_trace(uint R, uint Ki, out string trace)
        {
            // sum = (R + Ki) mod 2^32
            uint sum = unchecked(R + Ki);

            int[] nibbles = new int[8];
            for (int j = 0; j < 8; j++) nibbles[j] = (int)((sum >> (4 * j)) & 0xF);

            int[] svals = new int[8];
            uint sCombined = 0;
            for (int j = 0; j < 8; j++)
            {
                svals[j] = S[j, nibbles[j]];
                sCombined |= (uint)((uint)svals[j] << (4 * j));
            }

            uint f = Rotl32(sCombined, 11);

            // Формируем читаемый трейс
            trace = $"  sum = 0x{sum:X8}\n" +
                    $"  nibbles (j=7..0) = {string.Join(" ", Enumerable.Range(0, 8).Reverse().Select(j => nibbles[j].ToString("X")))}  (hex nibbles, j0 = least)\n" +
                    $"  svals  (j=7..0) = {string.Join(" ", Enumerable.Range(0, 8).Reverse().Select(j => svals[j].ToString("X")))}\n" +
                    $"  sCombined = 0x{sCombined:X8}\n" +
                    $"  f = ROTL11(sCombined) = 0x{f:X8}";

            return f;
        }
        public static byte[] Pad(byte[] data, int blockSize)
        {
            int padLen = blockSize - (data.Length % blockSize);
            if (padLen == 0) padLen = blockSize;
            byte[] res = new byte[data.Length + padLen];
            Buffer.BlockCopy(data, 0, res, 0, data.Length);
            for (int i = data.Length; i < res.Length; i++) res[i] = (byte)padLen;
            return res;
        }

        public static byte[] Unpad(byte[] data, int blockSize)
        {
            if (data.Length == 0 || data.Length % blockSize != 0) throw new ArgumentException("Invalid padded length");
            int padLen = data[data.Length - 1];
            if (padLen <= 0 || padLen > blockSize) throw new ArgumentException("Invalid padding length");
            for (int i = data.Length - padLen; i < data.Length; i++)
                if (data[i] != (byte)padLen) throw new ArgumentException("Invalid padding bytes");
            byte[] res = new byte[data.Length - padLen];
            Buffer.BlockCopy(data, 0, res, 0, res.Length);
            return res;
        }
        private static void ProcessBlock(byte[] inBlock, byte[] outBlock, uint[] roundKeys)
        {
            uint L = BytesToUInt32BE(inBlock, 0);
            uint R = BytesToUInt32BE(inBlock, 4);

            for (int i = 0; i < 32; i++)
            {
                string trace;
                uint f = F_with_trace(R, roundKeys[i], out trace);
                uint newR = L ^ f;
                L = R;
                R = newR;
            }

            UInt32ToBytesBE(R, outBlock, 0);
            UInt32ToBytesBE(L, outBlock, 4);
        }
        public EncryptionModel Encryption(EncryptionModel model)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(model.FileText ?? string.Empty);
            byte[] padded = Pad(bytes, 8);

            byte[] keyBytes = HexToBytes(model.PrimeP);
            if (keyBytes.Length != 32) throw new ArgumentException("Key must be 32 bytes.");
            uint[] K = new uint[8];
            for (int i = 0; i < 8; i++) K[i] = BytesToUInt32BE(keyBytes, i * 4);
            uint[] roundKeys = MakeRoundKeys(K);

            byte[] outBytes = new byte[padded.Length];
            byte[] inBlock = new byte[8];
            byte[] outBlock = new byte[8];

            switch (model.PrimeE)

            {
                case "ECB":
                    {
                        for (int pos = 0; pos < padded.Length; pos += 8)
                        {
                            Buffer.BlockCopy(padded, pos, inBlock, 0, 8);
                            ProcessBlock(inBlock, outBlock, roundKeys);
                            Buffer.BlockCopy(outBlock, 0, outBytes, pos, 8);
                        }
                    }
                    break;
                case "GAMMA":
                    {
                        byte[] gammaBlock = HexToBytes(model.PrimeQ);
                        for (int pos = 0; pos < padded.Length; pos += 8)
                        {
                            ProcessBlock(gammaBlock, gammaBlock, roundKeys); // шифруем gammaBlock

                            int blockSize = Math.Min(8, padded.Length - pos);
                            for (int i = 0; i < blockSize; i++)
                                outBytes[pos + i] = (byte)(padded[pos + i] ^ gammaBlock[i]);
                        }
                    }
                    break;
                case "CFB":
                    {
                        byte[] feedback = HexToBytes(model.PrimeQ);
                        byte[] gammaBlock = new byte[8];

                        for (int pos = 0; pos < padded.Length; pos += 8)
                        {
                            ProcessBlock(feedback, gammaBlock, roundKeys);

                            int blockSize = Math.Min(8, padded.Length - pos);
                            for (int i = 0; i < blockSize; i++)
                            {
                                outBytes[pos + i] = (byte)(padded[pos + i] ^ gammaBlock[i]);
                                feedback[i] = outBytes[pos + i]; // обновляем feedback
                            }
                        }
                    }
                    break;
            }

            return new EncryptionModel { FileText = BytesToHex(outBytes) };
        }

        public EncryptionModel Decryption(EncryptionModel model)
        {
            if (string.IsNullOrEmpty(model.FileText))
                return new EncryptionModel { FileText = string.Empty };

            string hex = model.FileText.Trim();
            if (hex.Length % 2 != 0) throw new ArgumentException("Invalid cipher hex length.");
            byte[] cipherBytes = HexToBytes(hex);

            byte[] keyBytes = HexToBytes(model.PrimeP);
            if (keyBytes.Length != 32) throw new ArgumentException("Key must be 32 bytes.");

            uint[] K = new uint[8];
            for (int i = 0; i < 8; i++) K[i] = BytesToUInt32BE(keyBytes, i * 4);

            // Ключи шифрования
            uint[] roundKeysEnc = MakeRoundKeys(K);

            // Ключи дешифрования (только для ECB)
            uint[] roundKeysDec = new uint[32];
            for (int i = 0; i < 32; i++) roundKeysDec[i] = roundKeysEnc[31 - i];

            if (cipherBytes.Length % 8 != 0) throw new ArgumentException("Cipher length must be multiple of 8 bytes.");

            byte[] outBytes = new byte[cipherBytes.Length];
            byte[] inBlock = new byte[8];
            byte[] outBlock = new byte[8];

            switch (model.PrimeE)
            {
                case "ECB":
                    for (int pos = 0; pos < cipherBytes.Length; pos += 8)
                    {
                        Buffer.BlockCopy(cipherBytes, pos, inBlock, 0, 8);
                        ProcessBlock(inBlock, outBlock, roundKeysDec);
                        Buffer.BlockCopy(outBlock, 0, outBytes, pos, 8);
                    }
                    break;

                case "GAMMA":
                    {
                        byte[] gammaBlock = HexToBytes(model.PrimeQ);
                        for (int pos = 0; pos < cipherBytes.Length; pos += 8)
                        {
                            ProcessBlock(gammaBlock, gammaBlock, roundKeysEnc); // генерация потока
                            int blockSize = Math.Min(8, cipherBytes.Length - pos);
                            for (int i = 0; i < blockSize; i++)
                                outBytes[pos + i] = (byte)(cipherBytes[pos + i] ^ gammaBlock[i]);
                        }
                    }
                    break;

                case "CFB":
                    {
                        byte[] feedback = HexToBytes(model.PrimeQ);
                        byte[] gammaBlock = new byte[8];

                        for (int pos = 0; pos < cipherBytes.Length; pos += 8)
                        {
                            ProcessBlock(feedback, gammaBlock, roundKeysEnc); // генерация потока
                            int blockSize = Math.Min(8, cipherBytes.Length - pos);

                            for (int i = 0; i < blockSize; i++)
                            {
                                byte cipherByte = cipherBytes[pos + i];
                                outBytes[pos + i] = (byte)(cipherByte ^ gammaBlock[i]);
                                feedback[i] = cipherByte; // обновляем feedback
                            }
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException($"Decryption mode '{model.PrimeE}' is not implemented yet.");
            }

            outBytes = Unpad(outBytes, 8);
            string resultDecryption = Encoding.UTF8.GetString(outBytes);
            return new EncryptionModel { FileText = resultDecryption };
        }
    }
}
