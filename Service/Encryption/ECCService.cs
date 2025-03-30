using System.Numerics;
using Domain.IService.IEncryption;
using Domain.Model;
using Domain;

namespace Service.Encryption;

public class ECCService : IECCService
{
    public class ECCParameters //P-192, кривая рекомендуемая NIST
    {
        public static readonly BigInteger p = BigInteger.Parse("6277101735386680763835789423207666416083908700390324961279"); //Модуль
        public static readonly BigInteger a = BigInteger.Parse("-3"); //Параметр 1
        public static readonly BigInteger b = BigInteger.Parse("2455155546008943817740293915197451784769108058161191238065"); //Параметр 2
        public static readonly BigInteger n = BigInteger.Parse("6277101735386680763835789423176059013767194773182842284081"); //Порядок
        public static readonly (BigInteger x, BigInteger y) G = (
            BigInteger.Parse("602046282375688656758213480587526111916698976636884684818"), // x координата
            BigInteger.Parse("174050332293622031404857552280219410364023488927386650641")  // y координата
        );
    }
    public EncryptionModel Encryption(EncryptionModel model)
    {
       var textCharArray = model.FileText.ToUpper().ToCharArray();
       var textStringArray = new string[textCharArray.Length];
        Random random = new Random();
        BigInteger Qx;
        if (!string.IsNullOrWhiteSpace(model.PrimeE))
        {
            Qx = BigInteger.Parse(model.PrimeE);
        }
        else
        {
            BigInteger d = BigInteger.Parse(model.CloseKeyD);
            Qx = d * ECCParameters.G.x;
        }
        for (int i = 0; i < textCharArray.Length; i++)
        {
            for (int j = 0; j < Settings.UkrainianAlphabet.Length; j++)
            {
                if (textCharArray[i] == Settings.UkrainianAlphabet[j])
                {
                    BigInteger k = random.Next(1, Int32.MaxValue);
                    BigInteger P1 = k * ECCParameters.G.x;
                    BigInteger P2 = k * Qx;
                    BigInteger Result = j + P2;
                    string encryptedSymbol = $"{P1},{Result}";
                    textStringArray[i] = encryptedSymbol.ToString();
                }
            }
        }
        var resultEncryption = string.Join("&", textStringArray);
        return new EncryptionModel
        {
            FileText = resultEncryption,
            PrimeE = Qx.ToString(),
        };
    }

    public EncryptionModel Decryption(EncryptionModel model)
    {
        var textStringArray = model.FileText.Split('&');
        var resultCharArray = new char[textStringArray.Length];
        BigInteger d = BigInteger.Parse(model.CloseKeyD);
        for (int i = 0; i < textStringArray.Length; i++)
        {
            string P1 = "";
            string C1 = "";
            string temp = textStringArray[i];
            int commaIndex = temp.IndexOf(',');
            if (commaIndex != -1)
            {
                P1 = temp.Substring(0, commaIndex);
                C1 = temp.Substring(commaIndex + 1);
            }
            if (P1 == "" || C1 == "")
            {
                resultCharArray[i] = '\n';
                continue;
            }
            BigInteger value_P1 = BigInteger.Parse(P1);
            BigInteger Value_C1 = BigInteger.Parse(C1);
            BigInteger S = d * value_P1;
            BigInteger index = Value_C1 - S;
            if (index >= Settings.UkrainianAlphabet.Length)
                index = 37;
            resultCharArray[i] = Settings.UkrainianAlphabet[(int)index];
        }
        string resultEncryption = string.Join("", resultCharArray);
        return new EncryptionModel
        {
            FileText = resultEncryption,
        };
    }

    public EncryptionModel Signature(EncryptionModel model)
    {
        BigInteger r, s, Q;
        if (string.IsNullOrWhiteSpace(model.PrimeE))
        {
            byte[] hash = Settings.HashMessageSHA1(model.FileText);
            Random random = new Random();
            BigInteger h = new BigInteger(hash);
            BigInteger d = BigInteger.Parse(model.CloseKeyD);
            Q = d * ECCParameters.G.x;
            BigInteger k = random.Next(1, Int32.MaxValue);
            BigInteger kInverse = BigInteger.ModPow(k, ECCParameters.n - 2, ECCParameters.n);
            BigInteger R = k * ECCParameters.G.x;
            r = R % ECCParameters.n;
            s = (kInverse * (h + d * r)) % ECCParameters.n;
            var signature = $"{model.FileText}#Цифровий підпис: {r.ToString()},{s.ToString()}";
            return new EncryptionModel
            {
                FileText = signature,
                PrimeE = Q.ToString()
            };
        }
        int Index = model.FileText.IndexOf('#');
        if (Index != -1)
        {
            Q = BigInteger.Parse(model.PrimeE);
            var signature = model.FileText.Substring(Index + 1);
            model.FileText = model.FileText.Substring(0, Index);
            byte[] hash = Settings.HashMessageSHA1(model.FileText);
            BigInteger h = new BigInteger(hash);
            string signatureString = signature.Replace("Цифровий підпис: ", "");
            int IndexKoma = signatureString.IndexOf(',');
            s = BigInteger.Parse(signatureString.Substring(IndexKoma + 1));
            r = BigInteger.Parse(signatureString.Substring(0, IndexKoma));
            BigInteger w = BigInteger.ModPow(s, ECCParameters.n - 2, ECCParameters.n);
            BigInteger u1 = h * w % ECCParameters.n;
            BigInteger u2 = r * w % ECCParameters.n;
            BigInteger P = u1 * ECCParameters.G.x + u2 * Q;
            BigInteger resultTestSignature = P % ECCParameters.n;
            if (r == resultTestSignature)
                return new EncryptionModel
                {
                    Signature = true
                };
            return new EncryptionModel
            {
                Signature = false
            };
        }
        return null;
    }
}