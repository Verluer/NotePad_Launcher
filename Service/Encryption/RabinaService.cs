using Domain.IService.IEncryption;
using Domain.Model;
using System.Numerics;
using Domain;
using Domain.Attributes;
using Domain.IService.ISystemApp;
using Microsoft.Extensions.DependencyInjection;
namespace Service.Encryption;

[RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IRabinaService))]
public class RabinaService : IRabinaService
{
    public EncryptionModel Encryption(EncryptionModel model)
    {
        var textCharArray = model.FileText.ToUpper().ToCharArray();
        var textStringArray = new string[textCharArray.Length];
        int value_p = 0, value_q = 0;
        int value_n;
        if (!string.IsNullOrWhiteSpace(model.ModulusN))
        {
            value_n = int.Parse(model.ModulusN);
        }
        else
        {
            value_p = int.Parse(model.PrimeP);
            value_q = int.Parse(model.PrimeQ);
            if (value_p % 4 != 3 || value_q % 4 != 3)
            {
                return new EncryptionModel()
                {
                    Signature = false //Использую как проверку корректно ввода числа p,q 
                };
            }
            value_n = value_p * value_q;
        }
        for (int i = 0; i < textCharArray.Length; i++)
        {
            for (int j = 0; j < Settings.UkrainianAlphabet.Length; j++)
            {
                if (textCharArray[i] == Settings.UkrainianAlphabet[j])
                {
                    BigInteger encryptedSymbol = BigInteger.ModPow(j, 2, value_n);
                    string encryptedString = encryptedSymbol.ToString();
                    textStringArray[i] = encryptedString;
                    break;

                }
            }
        }
        var resultEncryption = string.Join("&", textStringArray);
        return new EncryptionModel
        {
            FileText = resultEncryption,
            PrimeP = value_p.ToString(),
            PrimeQ = value_q.ToString(),
            ModulusN = value_n.ToString(),
            Signature = true //Использую как проверку корректно ввода числа p,q 
        };
    }

    public EncryptionModel Decryption(EncryptionModel model)
    {
        var textStringArray = model.FileText.Split('&');
        var resultCharArray = new char[textStringArray.Length];
        var value_p = int.Parse(model.PrimeP);
        var value_q = int.Parse(model.PrimeQ);
        var value_n = value_p * value_q;
        var backvalue_q = 0;
        var backvalue_p = 0;
        while (true)
        {
            backvalue_q++;
            if (value_q * backvalue_q % value_p == 1)
            {
                break;
            }
        }
        while (true)
        {
            backvalue_p++;
            if (value_p * backvalue_p % value_q == 1)
            {
                break;
            }
        }


        for (int i = 0; i < textStringArray.Length; i++)
        {
            BigInteger r1 = BigInteger.ModPow(int.Parse(textStringArray[i]), ((value_p + 1) / 4), value_p);
            BigInteger r2 = BigInteger.ModPow(int.Parse(textStringArray[i]), ((value_q + 1) / 4), value_q);

            int negative_r1 = (value_p - (int)r1) % value_p;
            int negative_r2 = (value_q - (int)r2) % value_q;
            int index1 = ((int)r1 * value_q * backvalue_q + (int)r2 * value_p * backvalue_p) % value_n;
            int index2 = ((int)r1 * value_q * backvalue_q + negative_r2 * value_p * backvalue_p) % value_n;
            int index3 = (negative_r1 * value_q * backvalue_q + (int)r2 * value_p * backvalue_p) % value_n;
            int index4 = (negative_r1 * value_q * backvalue_q + negative_r2 * value_p * backvalue_p) % value_n;
            if (index1 >= Settings.UkrainianAlphabet.Length)
                index1 = 37;
            if (index2 >= Settings.UkrainianAlphabet.Length)
                index2 = 37;
            if (index3 >= Settings.UkrainianAlphabet.Length)
                index3 = 37;
            if (index4 >= Settings.UkrainianAlphabet.Length)
                index4 = 37;
            string encryptedResult = $"Варианты символа {i}: {Settings.UkrainianAlphabet[(int)index1]}, {Settings.UkrainianAlphabet[(int)index2]}, {Settings.UkrainianAlphabet[(int)index3]}, {Settings.UkrainianAlphabet[(int)index4]} ";
            textStringArray[i] = encryptedResult;

        }
        var resultEncryption = string.Join(Environment.NewLine, textStringArray);
        return new EncryptionModel
        {
            FileText = resultEncryption
        };
    }
}