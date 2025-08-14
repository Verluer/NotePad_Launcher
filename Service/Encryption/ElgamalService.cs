using Domain.IService.IEncryption;
using Domain.Model;
using System.Numerics;
using Domain;
using Domain.Attributes;
using Domain.IService.ISystemApp;
using Microsoft.Extensions.DependencyInjection;
namespace Service.Encryption;

[RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IElgamalService))]
public class ElgamalService : IElgamalService
{
    public EncryptionModel Encryption(EncryptionModel model)
    {
        var textCharArray = model.FileText.ToUpper().ToCharArray();
        var textStringArray = new string[textCharArray.Length];
        Random random = new Random();
        int value_p, value_g, value_x = 0;
        BigInteger value_y;
        if (!string.IsNullOrWhiteSpace(model.PrimeE))
        {
            value_y = int.Parse(model.PrimeE);
            value_g = int.Parse(model.PrimeQ);
            value_p = int.Parse(model.PrimeP);
        }
        else
        {
            value_p = int.Parse(model.PrimeP); //Простое число
            value_g = int.Parse(model.PrimeQ); //Первообразный корень
            value_x = random.Next(2, value_p - 2); //Закрытый ключ 
            value_y = BigInteger.ModPow(value_g, value_x, value_p); //Открытый ключ y

        }
        var value_k = random.Next(2, value_p - 2); //Случайное число k
        BigInteger value_c1 = BigInteger.ModPow(value_g, value_k, value_p);

        for (int i = 0; i < textCharArray.Length; i++)
        {
            for (int j = 0; j < Settings.UkrainianAlphabet.Length; j++)
            {
                if (textCharArray[i] == Settings.UkrainianAlphabet[j])
                {
                    BigInteger encryptedSymbol = (BigInteger.Pow(value_y, value_k) * j) % value_p;
                    string encryptedStringSymbol = $"{value_c1},{encryptedSymbol}";
                    textStringArray[i] = encryptedStringSymbol;
                    break;
                }
            }
        }
        string resultEncryption = string.Join("&", textStringArray);
        return new EncryptionModel
        {
            FileText = resultEncryption,
            PrimeP = value_p.ToString(),
            PrimeQ = value_g.ToString(),
            PrimeE = value_y.ToString(),
            CloseKeyD = value_x.ToString()
        };

    }

    public EncryptionModel Decryption(EncryptionModel model)
    {
        string c1 = "";
        for (int i = 0; i < model.FileText.Length; i++)
        {
            if (model.FileText[i] == ',')
            {
                c1 = model.FileText.Substring(0, i);
                break;
            }
        }
        int value_c1 = int.Parse(c1);
        string newdata = model.FileText.Replace($"{c1},", "");
        var textStringArray = newdata.Split('&');
        var resultCharArray = new char[textStringArray.Length];
        int value_x = int.Parse(model.CloseKeyD);
        int value_p = int.Parse(model.PrimeP);
        int value_g = int.Parse(model.PrimeQ);
        BigInteger value_s = BigInteger.ModPow(value_c1, value_x, value_p);
        int backvalue_s = 0;
        while (true)
        {
            backvalue_s++;
            if (value_s * backvalue_s % value_p == 1)
            {
                break;
            }
        }
        for (int i = 0; i < textStringArray.Length; i++)
        {
            int decryptedResult = (int.Parse(textStringArray[i]) * backvalue_s) % value_p;
            resultCharArray[i] = Settings.UkrainianAlphabet[decryptedResult];
        }
        string ResultDecryption = string.Join("", resultCharArray);
        return new EncryptionModel
        {
            FileText = ResultDecryption
        };
    }
}