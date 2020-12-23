// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 08.07.2020 11:15
// Entwickler      Gregor Faiman
// Projekt         SIKOSI
namespace SRP_SDK
{
    using System;
    using System.Numerics;
    using System.Text;
    using System.Linq;
    using System.Security.Cryptography;
    using Org.BouncyCastle.Utilities;
    using Org.BouncyCastle.Security;

    /// <summary>
    /// Holds extension methods revolving around big integers as well as helper methods used for the SRP protocol.
    /// </summary>
    public static class ExtensionsAndHelpers
    {
        /// <summary>
        /// Converts a hexadecimal string into bytes.
        /// </summary>
        /// <param name="hexRepresentation">Die Hex-Darstellung der bytes.</param>
        /// <returns>Das umgewandelte byte array.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the input string is not a valid hex string and therefore can not be converted.
        /// </exception>
        public static byte[] ToByteArray(this string hexRepresentation)
        {
            if (hexRepresentation == null || hexRepresentation.Length % 2 != 0)
            {
                throw new ArgumentException(nameof(hexRepresentation), "input string was invalid");
            }

            var hexToByte = new byte[hexRepresentation.Length / 2];

            for (int i = 0; i < hexRepresentation.Length; i += 2)
            {
                hexToByte[i / 2] = Convert.ToByte(hexRepresentation.Substring(i, 2), 16);
            }

            return hexToByte;
        }

        /// <summary>
        /// Converts a byte array into a hexadecimal string representation.
        /// </summary>
        /// <param name="byteArray">The input byte array.</param>
        /// <returns>The hex string representing the byte array.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if byte array is null.
        /// </exception>
        public static string ToHexString(this byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException(nameof(byteArray), "byte array must not be null");

                StringBuilder hex = new StringBuilder(byteArray.Length * 2);
                
            foreach (byte b in byteArray)
                    hex.AppendFormat("{0:x2}", b);
             
            return hex.ToString();
        }

        /// <summary>
        /// Converts a byte array to a big integer.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>The big integer representation of the byte array.</returns>
        public static BigInteger ToBigInteger(this byte[] bytes)
        {
            return new BigInteger(bytes.Concat(new byte[1]).ToArray());
        }

        /// <summary>
        /// Converts a string representing a hex value to a big integer.
        /// </summary>
        /// <returns>The converted string.</returns>
        public static BigInteger ToBigInteger(this string hexRepresentation)
        {
            return BigInteger.Parse("0" + hexRepresentation, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Computes the value K that is requried to calculate the session keys for both client and server.
        /// </summary>
        /// <param name="generator">The used generator as defined by RFC5054. See: https://tools.ietf.org/html/rfc5054#appendix-A</param>
        /// <param name="N">The used value N, as defined by RFC5054. See: https://tools.ietf.org/html/rfc5054#appendix-A</param>
        /// <param name="hashFunction">The used hash function for computing the value K.</param>
        /// <returns>The computed value K.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if N or the hash function are null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if generator is negative.
        /// </exception>
        public static BigInteger ComputeK(SRPGroup srpGroup, HMAC hashFunction)
        {
            if (srpGroup == null)
                throw new ArgumentNullException(nameof(srpGroup), "SRP group must not be null.");

            if (hashFunction == null)
                throw new ArgumentNullException(nameof(hashFunction), "Hash function must not be null.");

            var NBytes = srpGroup.N.ToByteArray();
            var gBytes = PadBytes(BitConverter.GetBytes(srpGroup.Generator).Reverse().ToArray(), NBytes.Length);

            var k = hashFunction.ComputeHash(NBytes.Concat(gBytes).ToArray());

            return new BigInteger(k);
        }

        /// <summary>
        /// Pads bytes to the right.
        /// </summary>
        /// <param name="bytes">the byte array.</param>
        /// <param name="length">The padding length.</param>
        /// <returns>The padded byte array.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if bytes is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if length is negative.
        /// </exception>
        public static byte[] PadBytes(byte[] bytes, int length)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), "Bytes must not be null.");

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must not be negative.");

            var paddedBytes = new byte[length];
            Array.Copy(bytes, 0, paddedBytes, length - bytes.Length, bytes.Length);

            return paddedBytes;
        }

        /// <summary>
        /// Generates the random scrambling parameter U.
        /// </summary>
        /// <param name="hashFunction">The used hash function.</param>
        /// <param name="clientPublicValue">The public value A, generated by the client.</param>
        /// <param name="serverPublicValue">The public value B, generated by the server.</param>
        /// <returns>The random scrambling parameter U.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public static BigInteger ComputeU(HMAC hashFunction, BigInteger clientPublicValue, BigInteger serverPublicValue)
        {
            if (hashFunction == null)
                throw new ArgumentNullException(nameof(hashFunction), "hash function must not be null.");

            if (clientPublicValue == null)
                throw new ArgumentNullException(nameof(clientPublicValue), "Client public value must not be null.");

            if (serverPublicValue == null)
                throw new ArgumentNullException(nameof(serverPublicValue), "Server public value must not be null.");

            return hashFunction.ComputeHash(clientPublicValue.ToByteArray()
                .Concat(serverPublicValue.ToByteArray())
                .ToArray())
                .Concat(new byte[1])
                .ToArray()
                .ToBigInteger();
        }

        /// <summary>
        /// Custom modulo method.
        /// </summary>
        /// <param name="x">The original number.</param>
        /// <param name="m">The modulo.</param>
        /// <returns>The calculated modulo.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either x or a are negative.
        /// </exception>
        public static BigInteger Mod(BigInteger x, BigInteger m)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x), "x must not be null.");

            if (m == null)
                throw new ArgumentNullException(nameof(m), "Modulo must not be null.");

            BigInteger remainder = BigInteger.ModPow(x, 1, m);

            return remainder < 0 ? remainder + m : remainder;
        }

        /// <summary>
        /// Generates a random ephemeral value.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if SRP group is null.
        /// </exception>
        public static BigInteger GenerateEphemeralPrivateValue(SRPGroup srpGroup)
        {
            if (srpGroup == null)
                throw new ArgumentNullException(nameof(srpGroup), "SRP group must not be null.");

            int minBitAmount;

            BigInteger minimum;
            BigInteger maximum;
            BigInteger convertedPrivateValue;

            var random = new SecureRandom();

            Org.BouncyCastle.Math.BigInteger minimumToBouncyCastleInteger;
            Org.BouncyCastle.Math.BigInteger maximumToBouncyCastleInteger;
            Org.BouncyCastle.Math.BigInteger generatedPrivateValue;

            minBitAmount = Math.Min(256, srpGroup.N.ToByteArray().Length / 2);
            minimum = PadBytes(BigInteger.One.ToByteArray(), minBitAmount - 1).ToBigInteger();
            maximum = BigInteger.Subtract(srpGroup.N, BigInteger.One).ToByteArray().ToBigInteger();

            minimumToBouncyCastleInteger = new Org.BouncyCastle.Math.BigInteger(minimum.ToString());
            maximumToBouncyCastleInteger = new Org.BouncyCastle.Math.BigInteger(maximum.ToString());

            generatedPrivateValue = BigIntegers.CreateRandomInRange(minimumToBouncyCastleInteger, maximumToBouncyCastleInteger, random);

            convertedPrivateValue = new BigInteger(generatedPrivateValue.ToByteArray().Concat(new byte[1]).ToArray());

            return convertedPrivateValue;
        }
    }
}
