// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        12.05.2020 20:36
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

namespace SIKOSI.Crypto.Interfaces
{
    /// <summary>
    /// <para>
    ///Interface for the Diffie Hellman key exchange.
    /// </para>
    /// Interface IDiffieHellmanKeyExchange. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public interface IDiffieHellmanKeyExchange
    {
        /// <summary>
        /// The public key in the Diffie Hellman key exchange protocol.
        /// </summary>
        byte[] PublicKey { get; }

        /// <summary>
        /// Initializes a new key pair (private/public key) for the Diffie Hellman key exchange.
        /// </summary>
        void InitNewKeys();

        /// <summary>
        /// Computes the shared secret key in the Diffie Hellman key exchange protocol.
        /// </summary>
        /// <param name="otherPublicKey">The public key of another communication partner.</param>
        /// <returns>The secret key shared with the communication partner.</returns>
        byte[] GetSharedSecretKey(byte[] otherPublicKey);
    }
}