using System.Text.RegularExpressions;

class VignereCypher
{
    public static int Main()
    {
        string message = "tobeornottobethatisthequestionwhethertisnoblerinthemindtosuffertheslingsandarrowsofoutrageousfortuneortotakearmsagainstaseaoftroublesandbyopposingendthem";
        string keyPhrase = "cleanse thy holy land";
        string vigenereKey = "uncopyrightable"; //Should not have duplicate characters

        //Strip whitespace
        message = Regex.Replace(message, @"\s+", "");
        keyPhrase = Regex.Replace(keyPhrase, @"\s+", "");

        //Generate table
        string[] table = GenerateVigenereTable(vigenereKey);

        //Adjust key to match message length
        string keystream = MatchStringLength(message, keyPhrase);

        //Encrypt
        string encrypted = EncryptMessage(message, keystream, table);

        //Decrypt
        string decrypted = DecryptMessage(encrypted, keystream, table);



        //Print all results
        Console.WriteLine("ORIGINAL: " + message);
        Console.WriteLine("ENCRYPTED: " + encrypted);
        Console.WriteLine("DECRYPTED: " + decrypted);

        return 0;
    }

    public static string[] GenerateVigenereTable(string keyword)
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";

        //Filter key characters from alphabet
        foreach (char c in keyword)
        {
            alphabet = alphabet.Replace(c.ToString(), "");
        }

        //Create keyedAlphabet
        string keyedAlphabet = keyword + alphabet;


        //Create table
        string[] table = new string[keyedAlphabet.Length];

        //Load initial string in
        table[0] = keyedAlphabet;

        //Each row, shift the previous row left once
        for (int i = 1; i < keyedAlphabet.Length; i++)
        {
            keyedAlphabet = keyedAlphabet.Substring(1) + keyedAlphabet[0];
            table[i] = keyedAlphabet;
        }

        return table;
    }

    public static string MatchStringLength(string toMatch, string unmatched)
    {
        string matched = unmatched;
        int i = 0;

        while (toMatch.Length > matched.Length)
        {
            if (i == unmatched.Length) i = 0;

            matched += unmatched[i];

            i++;
        }

        return matched;
    }

    public static string EncryptMessage(string message, string keystream, string[] vigenereTable)
    {
        //Chunk message and keystream into length 26 chunks
        string[] chunkedMessage = ChunkString(message, 26);
        string[] chunkedKeystream = ChunkString(keystream, 26);
        string encryptedMessage = "";

        //Encrypt message
        for (int i = 0; i < chunkedMessage.Length; i++)
        {
            for (int row = 0, col = 0, j = 0; j < chunkedMessage[i].Length; j++)
            {
                //Plaintext character
                for (int k = 0; k < vigenereTable.Length; k++)
                {
                    if (vigenereTable[k][0] == chunkedMessage[i][j])
                    {
                        row = k;
                        break;
                    }
                }

                //KeyStream character
                col = vigenereTable[0].IndexOf(chunkedKeystream[i][j]);

                //Add encrypted character
                encryptedMessage += vigenereTable[row][col];
            }
        }

        return encryptedMessage;
    }

    public static string DecryptMessage(string encrypted, string keystream, string[] vigenereTable)
    {
        //Chunk message and keystream into length 26 chunks
        string[] chunkedMessage = ChunkString(encrypted, 26);
        string[] chunkedKeystream = ChunkString(keystream, 26);
        string decryptedMessage = "";

        for (int i = 0; i < chunkedMessage.Length; i++)
        {
            for (int col = 0, row = 0, j = 0; j < chunkedMessage[i].Length; j++)
            {
                //KeyStream character column
                col = vigenereTable[0].IndexOf(chunkedKeystream[i][j]);

                //Find cypher characters row
                for (int k = 0; k < vigenereTable.Length; k++)
                {
                    if (chunkedMessage[i][j] == vigenereTable[k][col])
                    {
                        row = k;
                        break;
                    }
                }

                //Add decrypted character
                decryptedMessage += vigenereTable[row][0];
            }
        }

        return decryptedMessage;
    }

    public static string[] ChunkString(string text, int chunkSize)
    {
        int sections = (int)Math.Ceiling((double)text.Length / chunkSize);

        string[] cutString = new string[sections];

        for (int start, end, i = 0; i < sections; i++)
        {
            start = i * chunkSize;

            if (start + chunkSize < text.Length)
            {
                end = start + chunkSize;
            }
            else
            {
                end = text.Length;
            }

            cutString[i] = text.Substring(start, end - start);
        }

        return cutString;
    }
}