using Domain.Attributes;
using Domain.IService.IEncryption;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.Encryption
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(ILFSRService))]
    public class LFSRService : ILFSRService
    {
        private ulong state;       // текущее состояние регистра (n бит в младших разрядах)
        private ulong tapMask; // маска обратной связи (бит i = 1 => s_i участвует в XOR)
        private int n;    // длина регистра (в битах)
        private static int Parity(ulong x)
        {
            int parity = 0;
            while (x != 0)
            {
                parity ^= 1;
                x &= (x - 1);
            }
            return parity;
        }
        private void LFSR(ulong seed, ulong tapMaskNew, int length)
        {
            n = length;
            state = seed & ((n == 64) ? ulong.MaxValue : ((1UL << n) - 1)); // обрезаем до n бит
            tapMask = tapMaskNew & ((n == 64) ? ulong.MaxValue : ((1UL << n) - 1));
        }
        private byte GetNextKeyByte()
        {
            byte key = 0;
            for (int i = 0; i < 8; i++)
            {
                int outBit = (int)(state & 1UL);
                int feedback = Parity(state & tapMask);
                state = (state >> 1) | ((ulong)feedback << (n - 1));
                key |= (byte)(outBit << i); // LSB-first
            }
            return key;
        }
        public EncryptionModel Encryption(EncryptionModel model)
        {
            var textCharArray = model.FileText.ToCharArray();

            LFSR(Convert.ToUInt64(model.PrimeQ, 16), Convert.ToUInt64(model.PrimeE, 16), int.Parse(model.PrimeP));

            string[] resultArray = new string[textCharArray.Length];
            for(int i = 0; i < textCharArray.Length; i++)
            {
                byte encryption = (byte)(textCharArray[i] ^ GetNextKeyByte());
                resultArray[i] = encryption.ToString("X2");   
            }
            string resultEncryption = string.Join("&", resultArray);
            return new EncryptionModel
            {
                FileText = resultEncryption,
            };
        }

        public EncryptionModel Decryption(EncryptionModel model)
        {
            var textStringArray = model.FileText.Split('&');
            LFSR(Convert.ToUInt64(model.PrimeQ, 16), Convert.ToUInt64(model.PrimeE, 16), int.Parse(model.PrimeP));

            string[] resultArray = new string[textStringArray.Length];
            for (int i = 0; i < textStringArray.Length; i++)
            {
                byte decrypted = (byte)(Convert.ToByte(textStringArray[i], 16) ^ GetNextKeyByte());
                resultArray[i] = ((char)decrypted).ToString();
            }
            string resultEncryption = string.Concat(resultArray);
            return new EncryptionModel
            {
                FileText = resultEncryption,
            };
        }
    }
}
