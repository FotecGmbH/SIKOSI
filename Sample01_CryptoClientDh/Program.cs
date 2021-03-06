﻿// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        19.05.2020 15:25
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using SIKOSI.Crypto;
using SecurityDriven.Inferno.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample01_CryptoClientDh
{
    public class Program
    {
        /// <summary>
        /// The base address of the server.
        /// Sample01_CryptoServer must be up and running at the specified address
        /// and listening on the specified port.
        /// </summary>
        public static string ServerBaseAddress = "https://localhost:44312/";

        /// <summary>
        /// The HttpClient used for communication with the server.
        /// </summary>
        public static HttpClient Client = new HttpClient { BaseAddress = new Uri(ServerBaseAddress) };

        /// <summary>
        /// This sample program asks for a secret message and sends this message encrypted to the server using Diffie-Hellman key exchange and AES-GCM.
        /// The server answers whether the decryption and authentication was successfull.
        /// Sample01_CryptoServer must be up and running.
        /// </summary>
        /// <param name="args">Not used in this program.</param>
        /// <returns>A task.</returns>
        public static async Task Main(string[] args)
        {
            // Asking for message to encrypt
            Console.Write("Enter a secret message to be encrypted using Diffie Hellman key exchange and to be sent to the server: ");
            var secretMessage = Console.ReadLine();

            /* Get the public key of the server or any other communication partner.
             * This key is used for calculating the symmetric key during the Diffie-Hellman key exchange
             * */
            var serverPublicKey = await GetServerPublicKey();

            // Encrypt the message
            var encryption = new SecureEncryptionDh();
            var cryptoResult = encryption.EncryptData(serverPublicKey, secretMessage.ToBytes());

            // If encryption is successfull - send the encrypted byte array to the server or any other communication partner.
            if (cryptoResult.Success)
            {
                var response = Client.PostAsync("api/crypto/dh/postmessage", new ByteArrayContent(cryptoResult.ResultBytes)).Result;

                /* Print a message whether the server sent a success code.
                 * That means whehter the server was able to extract, decrypt and authenticate the received bytes.
                 * */
                Console.WriteLine(response.IsSuccessStatusCode ? 
                    "Encrypted message sent and decrypted successfully." : "Sending or decrypting failed.");
            }
            else
            {
                Console.WriteLine("Encryption failed!");
                Console.WriteLine(cryptoResult.CausingException?.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Gets the public key of the server for the Diffie-Hellman key exchange.
        /// Sample01_CryptoServer must be up and running.
        /// </summary>
        /// <returns></returns>
        private static async Task<byte[]> GetServerPublicKey()
        {
            return await Client.GetByteArrayAsync("api/crypto/dh/publickey");
        }
    }
}
