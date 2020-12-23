// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 16.09.2020 11:24
// Entwickler      Gregor Faiman
// Projekt         SIKOSI
namespace Sample01_SRPClient
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point. Used to demonstrate how to use the demo project. This it not good code, and serves the only purpose
        /// of providing a test case for SRP.
        /// </summary>
        /// <param name="args">Unused argument.</param>
        /// <returns>An empty task.</returns>
        public static async Task Main(string[] args)
        {
            // This demo project uses http requests as the chosen means of network communication.
            // However this SDK is not limited to the use case of HTTP requests.

            var initialWidth = Console.WindowWidth;
            var initialHeight = Console.WindowHeight;

            Console.SetWindowSize(Console.LargestWindowWidth - 5, Console.LargestWindowHeight - 5);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            var application = new Application();
            await application.Start();

            Console.SetWindowSize(initialWidth, initialHeight);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
    }
}
