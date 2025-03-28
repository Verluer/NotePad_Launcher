using Domain.IService.IEncryption;
using Domain.Model;
using System.Numerics;
using Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Encryption;

public class RSAService : IRSAService
{
    public EncryptionModel Encryption(EncryptionModel model)
    {
        var textCharArray = model.FileText.ToUpper().ToCharArray();
        var textIntArray = new int[textCharArray.Length];
        BigInteger modulus_n, value_e, d = 0;
        if (!string.IsNullOrEmpty(model.ModulusN))
        {
            value_e = int.Parse(model.PrimeE);
            modulus_n = int.Parse(model.ModulusN);
        }
        else
        {
            var value_p = BigInteger.Parse(model.PrimeP);
            var value_q = BigInteger.Parse(model.PrimeQ);
            modulus_n = value_p * value_q;
            var fn = (value_p - 1) * (value_q - 1);
            value_e = BigInteger.Parse(model.PrimeE);

            if (value_e > 0 && value_e < modulus_n)
            {
                var k = 0;
                while (true)
                {
                    k++;
                    if ((k * fn + 1) % value_e != 0) continue;
                    d = (k * fn + 1) / value_e;
                    break;
                }
            }
        }
        for (int i = 0; i < textCharArray.Length; i++)
        {
            for (int j = 0; j < Settings.UkrainianAlphabet.Length; j++)
            {
                if (textCharArray[i] != Settings.UkrainianAlphabet[j]) continue;
                var encryptedSymbol = BigInteger.ModPow(j, value_e, modulus_n);
                textIntArray[i] = (int)encryptedSymbol;
                break;
            }
        }
        var resultEncryption = string.Join("&", textIntArray);
        return new EncryptionModel
        {
            CloseKeyD = d.ToString(),
            PrimeE = value_e.ToString(),
            ModulusN = modulus_n.ToString(),
            FileText = resultEncryption
        };
    }

    public EncryptionModel Decryption(EncryptionModel model)
    {
        var textStringArray = model.FileText.Split('&');
        int value_d = int.Parse(model.CloseKeyD);
        int value_n = int.Parse(model.ModulusN);
        var encryptedNumbers = textStringArray.Select(int.Parse).ToArray();
        var resultCharArray = new char[encryptedNumbers.Length];
        for (int i = 0; i < encryptedNumbers.Length; i++)
        {
            int encryptedNumber = encryptedNumbers[i];
            BigInteger result = BigInteger.ModPow(encryptedNumber, value_d, value_n);
            resultCharArray[i] = Settings.UkrainianAlphabet[(int)result];
        }
        string resultEncryption = string.Join("", resultCharArray);
        return new EncryptionModel
        {
            FileText = resultEncryption
        };
    }

    public EncryptionModel Signature(EncryptionModel model)
    {
        var textCharArray = model.FileText.ToUpper().ToCharArray();

        string signature;
        BigInteger S = 0;
        if (!string.IsNullOrEmpty(model.ModulusN))
        {

            var modulus_n = BigInteger.Parse(model.ModulusN);
            var value_e = BigInteger.Parse(model.PrimeE);

            int Index = model.FileText.IndexOf('#');
            if (Index != -1)
            {
                signature = model.FileText.Substring(Index + 1);
                model.FileText = model.FileText.Substring(0, Index);
                S = BigInteger.Parse(signature.Replace("Цифровий підпис: ", ""));
            }
            textCharArray = model.FileText.ToUpper().ToCharArray();
            BigInteger resultSign = 0;
            for (int i = 0; i < textCharArray.Length; i++)
            {
                for (int j = 0; j < Settings.UkrainianAlphabet.Length; j++)
                {
                    if (textCharArray[i] == Settings.UkrainianAlphabet[j])
                    {
                        resultSign += j;
                        break;
                    }
                }
            }
            BigInteger HashM = BigInteger.ModPow(S, value_e, modulus_n);
            if (HashM == resultSign)
                model.Signature = true;
            else
                model.Signature = false;
            return new EncryptionModel
            {
                Signature = model.Signature,
            };
        }
        else
        {
            var value_p = BigInteger.Parse(model.PrimeP);
            var value_q = BigInteger.Parse(model.PrimeQ);
            var value_e = BigInteger.Parse(model.PrimeE);
            BigInteger d = default;
            var fn = (value_p - 1) * (value_q - 1);
            var modulus_n = value_p * value_q;
            if (value_e > 0 && value_e < modulus_n)
            {
                var k = 0;
                while (true)
                {
                    k++;
                    if ((k * fn + 1) % value_e != 0) continue;
                    d = (k * fn + 1) / value_e;
                    break;
                }
            }
            BigInteger resultSign = 0;
            for (int i = 0; i < textCharArray.Length; i++)
            {
                for (int j = 0; j < Settings.UkrainianAlphabet.Length; j++)
                {
                    if (textCharArray[i] == Settings.UkrainianAlphabet[j])
                    {
                        resultSign += j;
                        break;
                    }
                }
            }
            S = BigInteger.ModPow(resultSign, d, modulus_n);
            var signatureText = $"{model.FileText}#Цифровий підпис: {S.ToString()}";
            LogMessage(signatureText);
            return new EncryptionModel
            {
                FileText = signatureText
            };
        }
    }
    public void LogMessage(string message)
    {
        string logFilePath = "D:\\VIsual Studio\\VS project\\NotePad_Launcher\\NotePad_Launcher\\bin\\Debug\\net8.0-windows\\Documents\\log.txt";
        File.AppendAllText(logFilePath, DateTime.Now + ": " + message + Environment.NewLine);
    }
}