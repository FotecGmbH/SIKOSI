// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 08.07.2020 10:22
// Entwickler      Gregor Faiman
// Projekt         SIKOSI
namespace SRP_SDK
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Class containing information about the used SRP group.
    /// </summary>
    public class SRPGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SRPGroup"/> class, using the 2048 Bit group's values by default.
        /// The 2048 and other Bit groups can be found here at 
        /// https://tools.ietf.org/html/rfc5054#appendix-A.
        /// </summary>
        public SRPGroup()
        {
            this.Generator = 2;
            this.N = "AC6BDB41324A9A9BF166DE5E1389582FAF72B6651987EE07FC3192943DB56050A37329CBB4A099ED8193E0757767A13DD52312AB4B03310DCD7F48A9DA04FD50E8083969EDB767B0CF6095179A163AB3661A05FBD5FAAAE82918A9962F0B93B855F97993EC975EEAA80D740ADBF4FF747359D041D5C33EA71D281E446B14773BCA97B43A23FB801676BD207A436C6481F1D2B9078717461A5B9D32E688F87748544523B524B0D57D5EA77A2775D2ECFA032CFBDBF52FB3786160279004E57AE6AF874E7303CE53299CCC041C7BC308D82A5698F3A8D0C38271AE35F8E9DBFBB694B5C803D89F7AE435DE236D525F54759B65E372FCD68EF20FA7111F9E4AFF73".ToBigInteger();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SRPGroup"/> class.
        /// Use https://tools.ietf.org/html/rfc5054#appendix-A to choose a SRP group.
        /// This must be prematurely agreed upon by both the server and the client.
        /// </summary>
        /// <param name="generator">The used generator.</param>
        /// <param name="N">The used value N.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if generator or N are negative.
        /// </exception>
        public SRPGroup(int generator, BigInteger N)
        {
            if (generator < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(generator), "Generator must not be negative");
            }

            if (N < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(N), "N must not be negative");
            }

            this.Generator = generator;
            this.N = N;
        }

        /// <summary>
        /// Gets the used generator.
        /// </summary>
        /// <value>The used generator.</value>
        public int Generator
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the used value N.
        /// </summary>
        /// <value>The used value N.</value>
        public BigInteger N
        {
            get;
            private set;
        }
    }
}
