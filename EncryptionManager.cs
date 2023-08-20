/*
 * Copyright (c) 2023 Darshil "Dark9" Patel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 * Hari Om Tatsat
 */
using System;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class EncryptionManager : MonoBehaviour
{
    public int value;
    public int[] addned;
    
    public void checkHonor() // for test
    {
        Debug.Log(ScoringHonorMaintained());
    }
    public void secureScore() // call after every change in score/playerprefs
    {
        saveNcheckScore(true); 
    }

    public bool ScoringHonorMaintained() // call to check if its same or not
    {
        return string.Equals(PlayerPrefs.GetString("ScoreHonor"), saveNcheckScore(false));
    }

    private string saveNcheckScore(bool save)
    {
        string TheEncrypt = ConvertToFixedSizeString(value, 9);
        foreach (int i in addned)
        {
            TheEncrypt = TheEncrypt + ConvertToFixedSizeString(value, i);
        }
        Debug.Log("string:" + TheEncrypt);
        Debug.Log("Hash:" + ComputeSHA256Hash(TheEncrypt));

        if(save) PlayerPrefs.SetString("ScoreHonor", ComputeSHA256Hash(TheEncrypt));

        return ComputeSHA256Hash(TheEncrypt);

    }

    private string ConvertToFixedSizeString(int value, int addend)
    {
        // Convert the integer to a fixed-length bit string
        string bitString = Convert.ToString(value, 2).PadLeft(32, '0');
        string fixedString = "";

        // Convert four 8-bit chunks to their hexadecimal equivalent
        for (int i = 0; i < bitString.Length; i += 8)
        {
            fixedString += Convert.ToByte(bitString.Substring(i, 8), 2).ToString("X2");
        }

        // Perform additive ciphering on each character in the string
        char[] charArray = fixedString.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            int charValue = Convert.ToInt32(charArray[i].ToString(), 16);
            if (i % 2 == 0)
            {
                charValue = (charValue + addend) % 16;
            }
            else
            {
                charValue = (charValue + 16 - addend) % 16;
            }
            charArray[i] = charValue.ToString("X")[0];
        }
        fixedString = new string(charArray);

        return fixedString;
    }

    private string ComputeSHA256Hash(string input)
    {
        using (SHA256 sha256hash = SHA256.Create())
        {
            byte[] data = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(data);
        }
    }

}