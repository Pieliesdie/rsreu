﻿using System;
using System.Linq;

namespace AES
{
    public class AesCryp
    {
        private int Nk = 4; // length of key 4xNk, bytes
        private int Nb = 4; // size of block, bytes
        private int Nr = 10; // count of rounds, bytes
        private byte[] RoundKeys;

        public AesCryp(byte[] secretKey)
        {
            RoundKeys = KeyExpansion(secretKey);
        }

        private readonly byte[] sbox = { 0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F,
            0xC5, 0x30, 0x01, 0x67, 0x2B, 0xFE, 0xD7, 0xAB, 0x76, 0xCA, 0x82,
            0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 0xAD, 0xD4, 0xA2, 0xAF, 0x9C,
            0xA4, 0x72, 0xC0, 0xB7, 0xFD, 0x93, 0x26, 0x36, 0x3F, 0xF7, 0xCC,
            0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15, 0x04, 0xC7, 0x23,
            0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 0xEB, 0x27,
            0xB2, 0x75, 0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 0x52,
            0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84, 0x53, 0xD1, 0x00, 0xED,
            0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58,
            0xCF, 0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9,
            0x02, 0x7F, 0x50, 0x3C, 0x9F, 0xA8, 0x51, 0xA3, 0x40, 0x8F, 0x92,
            0x9D, 0x38, 0xF5, 0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2,
            0xCD, 0x0C, 0x13, 0xEC, 0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E,
            0x3D, 0x64, 0x5D, 0x19, 0x73, 0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A,
            0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 0xDE, 0x5E, 0x0B, 0xDB, 0xE0,
            0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 0xC2, 0xD3, 0xAC, 0x62,
            0x91, 0x95, 0xE4, 0x79, 0xE7, 0xC8, 0x37, 0x6D, 0x8D, 0xD5, 0x4E,
            0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08, 0xBA, 0x78,
            0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 0x4B,
            0xBD, 0x8B, 0x8A, 0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E,
            0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E, 0xE1, 0xF8, 0x98,
            0x11, 0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55,
            0x28, 0xDF, 0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41,
            0x99, 0x2D, 0x0F, 0xB0, 0x54, 0xBB, 0x16 };

        private byte[] inv_sbox = { 0x52, 0x09, 0x6A, 0xD5, 0x30, 0x36, 0xA5,
            0x38, 0xBF, 0x40, 0xA3, 0x9E, 0x81, 0xF3, 0xD7, 0xFB, 0x7C, 0xE3,
            0x39, 0x82, 0x9B, 0x2F, 0xFF, 0x87, 0x34, 0x8E, 0x43, 0x44, 0xC4,
            0xDE, 0xE9, 0xCB, 0x54, 0x7B, 0x94, 0x32, 0xA6, 0xC2, 0x23, 0x3D,
            0xEE, 0x4C, 0x95, 0x0B, 0x42, 0xFA, 0xC3, 0x4E, 0x08, 0x2E, 0xA1,
            0x66, 0x28, 0xD9, 0x24, 0xB2, 0x76, 0x5B, 0xA2, 0x49, 0x6D, 0x8B,
            0xD1, 0x25, 0x72, 0xF8, 0xF6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xD4,
            0xA4, 0x5C, 0xCC, 0x5D, 0x65, 0xB6, 0x92, 0x6C, 0x70, 0x48, 0x50,
            0xFD, 0xED, 0xB9, 0xDA, 0x5E, 0x15, 0x46, 0x57, 0xA7, 0x8D, 0x9D,
            0x84, 0x90, 0xD8, 0xAB, 0x00, 0x8C, 0xBC, 0xD3, 0x0A, 0xF7, 0xE4,
            0x58, 0x05, 0xB8, 0xB3, 0x45, 0x06, 0xD0, 0x2C, 0x1E, 0x8F, 0xCA,
            0x3F, 0x0F, 0x02, 0xC1, 0xAF, 0xBD, 0x03, 0x01, 0x13, 0x8A, 0x6B,
            0x3A, 0x91, 0x11, 0x41, 0x4F, 0x67, 0xDC, 0xEA, 0x97, 0xF2, 0xCF,
            0xCE, 0xF0, 0xB4, 0xE6, 0x73, 0x96, 0xAC, 0x74, 0x22, 0xE7, 0xAD,
            0x35, 0x85, 0xE2, 0xF9, 0x37, 0xE8, 0x1C, 0x75, 0xDF, 0x6E, 0x47,
            0xF1, 0x1A, 0x71, 0x1D, 0x29, 0xC5, 0x89, 0x6F, 0xB7, 0x62, 0x0E,
            0xAA, 0x18, 0xBE, 0x1B, 0xFC, 0x56, 0x3E, 0x4B, 0xC6, 0xD2, 0x79,
            0x20, 0x9A, 0xDB, 0xC0, 0xFE, 0x78, 0xCD, 0x5A, 0xF4, 0x1F, 0xDD,
            0xA8, 0x33, 0x88, 0x07, 0xC7, 0x31, 0xB1, 0x12, 0x10, 0x59, 0x27,
            0x80, 0xEC, 0x5F, 0x60, 0x51, 0x7F, 0xA9, 0x19, 0xB5, 0x4A, 0x0D,
            0x2D, 0xE5, 0x7A, 0x9F, 0x93, 0xC9, 0x9C, 0xEF, 0xA0, 0xE0, 0x3B,
            0x4D, 0xAE, 0x2A, 0xF5, 0xB0, 0xC8, 0xEB, 0xBB, 0x3C, 0x83, 0x53,
            0x99, 0x61, 0x17, 0x2B, 0x04, 0x7E, 0xBA, 0x77, 0xD6, 0x26, 0xE1,
            0x69, 0x14, 0x63, 0x55, 0x21, 0x0C, 0x7D };

        public void SubBytes(byte[] state)
        {
            for(int i = 0; i < state.Length; i++)
            {
                state[i] = sbox[state[i]];
            }
        }

        public void InvSubBytes(byte[] state)
        {
            for (int i = 0; i < state.Length; i++)
            {
                state[i] =inv_sbox[state[i]];
            }
        }

        byte[] KeyExpansion(byte[] secretKey)
        {
            byte[] w = new byte[4 * Nk * (Nr + 1)];

            for (int i = 0; i < secretKey.Length; i++)
            {
                w[i] = secretKey[i];
            }

            for (int i = secretKey.Length; i < w.Length; i += 4)
            {               
                if (i % (Nk * 4) == 0)
                {
                    byte[] tmp = w.Skip((i-4)).Take(4).ToArray();
                    RotWord(tmp);
                    SubBytes(tmp);
                    XORRcon(tmp,i/16);
                    w[i] = (byte)(tmp[0] ^ w[i - 16]);
                    w[i+1] = (byte)(tmp[1] ^ w[i - 15]);
                    w[i+2] = (byte)(tmp[2] ^ w[i - 14]);
                    w[i+3] = (byte)(tmp[3] ^ w[i - 13]);
                }
                else
                {
                    w[i] = (byte)(w[i-4] ^ w[i - 16]);
                    w[i + 1] = (byte)(w[i-3] ^ w[i - 15]);
                    w[i + 2] = (byte)(w[i-2] ^ w[i - 14]);
                    w[i + 3] = (byte)(w[i-1] ^ w[i - 13]);
                }
            }
            return w;

            void XORRcon(byte[] array,int i)
            {
                array[0] = (byte)(array[0] ^ (byte)Math.Pow(2,i-1));
                array[1] = (byte)(array[1] ^ 0);
                array[2] = (byte)(array[2] ^ 0);
                array[3] = (byte)(array[3] ^ 0);
            }

            void RotWord(byte[] input)
            {
                byte tmp = input[0];
                input[0] = input[1];
                input[1] = input[2];
                input[2] = input[3];
                input[3] = tmp;
            }
        }

        public void ShiftRows(byte[] state)
        {
            void CircleMoveL(int k, int j, int n)
            {
                for (int count = 0; count < n; count++)
                {
                    byte tmp = state[k];
                    for (int i = k; i < j - 1; i++)
                    {
                        state[i] = state[i + 1];
                    }
                    state[j - 1] = tmp;
                }
            }
            void reverseArray()
            {
                byte[] tmp1 = new byte[state.Length];
                for (int i = 0, j = 0; i < state.Length; i += 4, j++)
                {
                    tmp1[j] = state[i];
                    tmp1[j + 4] = state[i + 1];
                    tmp1[j + 8] = state[i + 2];
                    tmp1[j + 12] = state[i + 3];
                }
                for (int i = 0; i < state.Length; i++)
                {
                    state[i] = tmp1[i];
                }
            }
            reverseArray();
            CircleMoveL(4, 8, 1);
            CircleMoveL(8, 12, 2);
            CircleMoveL(12, 16, 3);
            reverseArray();
        }

        public void InvShiftRows(byte[] state)
        {
            void CircleMoveL(int k, int j, int n)
            {
                for (int count = 0; count < n; count++)
                {
                    byte tmp = state[k];
                    for (int i = k; i < j - 1; i++)
                    {
                        state[i] = state[i + 1];
                    }
                    state[j - 1] = tmp;
                }
            }
            void reverseArray()
            {
                byte[] tmp1 = new byte[state.Length];
                for (int i = 0, j = 0; i < state.Length; i += 4, j++)
                {
                    tmp1[j] = state[i];
                    tmp1[j + 4] = state[i + 1];
                    tmp1[j + 8] = state[i + 2];
                    tmp1[j + 12] = state[i + 3];
                }
                for (int i = 0; i < state.Length; i++)
                {
                    state[i] = tmp1[i];
                }
            }
            reverseArray();
            CircleMoveL(4, 8, 3);
            CircleMoveL(8, 12, 2);
            CircleMoveL(12, 16, 1);
            reverseArray();
        }

        public void MixColumns(byte[] state)
        {
            for (int i = 0; i < state.Length; i += 4)
            {
                byte[] tmp = state.Skip(i).Take(4).ToArray();
                state[i] = (byte)(GMul(tmp[0], 2) ^ GMul(tmp[1], 3) ^ GMul(tmp[2],1) ^ GMul(tmp[3],1));
                state[i+1] = (byte)(GMul(tmp[0], 1) ^ GMul(tmp[1], 2) ^ GMul(tmp[2], 3) ^ GMul(tmp[3], 1));
                state[i+2] = (byte)(GMul(tmp[0], 1) ^ GMul(tmp[1], 1) ^ GMul(tmp[2], 2) ^ GMul(tmp[3], 3));
                state[i+3] = (byte)(GMul(tmp[0], 3) ^ GMul(tmp[1], 1) ^ GMul(tmp[2], 1) ^ GMul(tmp[3], 2));
            }
        }

        public void InvMixColumns(byte[] state)
        {
            for (int i = 0; i < state.Length; i += 4)
            {
                byte[] tmp = state.Skip(i).Take(4).ToArray();
                state[i] = (byte)(GMul(tmp[0], 0x0E) ^ GMul(tmp[1], 0x0B) ^ GMul(tmp[2], 0x0D) ^ GMul(tmp[3], 0x09));
                state[i + 1] = (byte)(GMul(tmp[0], 9) ^ GMul(tmp[1], 0x0E) ^ GMul(tmp[2], 0x0B) ^ GMul(tmp[3], 0x0D));
                state[i + 2] = (byte)(GMul(tmp[0], 0x0d) ^ GMul(tmp[1], 9) ^ GMul(tmp[2], 0x0E) ^ GMul(tmp[3], 0x0B));
                state[i + 3] = (byte)(GMul(tmp[0], 0x0B) ^ GMul(tmp[1], 0x0D) ^ GMul(tmp[2], 9) ^ GMul(tmp[3], 0x0E));
            }
        }

        private byte GMul(byte a, byte b)
        { // Galois Field (256) Multiplication of two Bytes
            byte p = 0;

            for (int counter = 0; counter < 8; counter++)
            {
                if ((b & 1) != 0)
                {
                    p ^= a;
                }

                bool hi_bit_set = (a & 0x80) != 0;
                a <<= 1;
                if (hi_bit_set)
                {
                    a ^= 0x1B; /* x^8 + x^4 + x^3 + x + 1 */
                }
                b >>= 1;
            }

            return p;
        }

        void AddRoundKey(byte[] state,int w)
        {
            byte[] roundkey = RoundKeys.Skip(w * 4*Nk).Take(4 * Nk).ToArray();

            for(int i =0; i < state.Length; i++)
            {
                state[i] =(byte)(state[i] ^ roundkey[i]);
            }
        }

        public byte[] Encrypt(byte[] state)
        {
            byte[] tmp = new byte[state.Length];
            state.CopyTo(tmp, 0);
            AddRoundKey(tmp, 0);
            for(int i = 1; i < Nr; i++)
            {
                SubBytes(tmp);
                ShiftRows(tmp);
                MixColumns(tmp);
                AddRoundKey(tmp, i);
            }
            SubBytes(tmp);
            ShiftRows(tmp);
            AddRoundKey(tmp, Nr);
            return tmp;
        }

        public byte[] Decrypt(byte[] state)
        {
            byte[] tmp = new byte[state.Length];
            state.CopyTo(tmp, 0);
            AddRoundKey(tmp, Nr);
            for (int i = Nr - 1; i >= 1; i--)
            {
                InvSubBytes(tmp);
                InvShiftRows(tmp);
                AddRoundKey(tmp, i);
                InvMixColumns(tmp);
            }
            InvSubBytes(tmp);
            InvShiftRows(tmp);
            AddRoundKey(tmp, 0);
            return tmp;
        }
    }
}