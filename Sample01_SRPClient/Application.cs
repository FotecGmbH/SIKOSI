// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        16.09.2020 11:25
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Sample01_SRPClient
{
    public class Application
    {
        private byte[] currentSessionKey;
        private bool isRunning;
        private SrpApplicationClient client;

        public Application()
        {
            this.Menu = new Menu(new List<MenuEntry>()
            {
                new MenuEntry("LogIn", this.Login),
                new MenuEntry("Register", this.Register),
                new MenuEntry("Send Message", this.StartSendMessages),
                new MenuEntry("Exit Demo", null)
            });
        }

        public Menu Menu
        {
            get;
            set;
        }

        public async Task Start()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|----------------------------------------|");
            Console.WriteLine("|   Welcome to this demo application.    |");
            Console.WriteLine("|----------------------------------------|");
            Console.WriteLine();
            Console.ResetColor();

            this.isRunning = true;
            await this.Run();
        }

        private async Task Run()
        {
            bool redraw = true;

            while (this.isRunning)
            {
                if (redraw)
                    this.DrawMenu();

                var input = Console.ReadKey(true).Key;

                redraw = await this.ExecuteInput(input);
            }

            Console.WriteLine("***Application terminated.***");
        }

        private async Task Login()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|----------------------------------------|");
            Console.WriteLine("|               Einloggen                |");
            Console.WriteLine("|----------------------------------------|");
            Console.WriteLine();
            Console.ResetColor();

            string username;
            string password;

            Console.Write("Wählen Sie einen Benutzernamen: ");
            username = this.GetString();

            Console.Write("Wählen Sie ein Passwort: ");
            password = this.GetString();

            this.client = new SrpApplicationClient(username, password, new SRP_SDK.SrpClient(new SRP_SDK.SRPGroup()), new Uri("https://localhost:44314/"));

            try
            {
                this.currentSessionKey = await client.LoginAsync($"api/user/getsalt/{username}", "api/user/login/postvalue");
                var isProofValid = await client.ComputeProof(currentSessionKey, $"api/user/proof/postproof");

                if (isProofValid)
                    Console.WriteLine("Benutzer eingeloggt. Bereit Nachrichten zu schicken. Bitte mit Tastendruck bestätigen.");
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                this.currentSessionKey = null;
                Console.WriteLine("Benutzer konnte nicht authentifiziert werden");
            }
            

            Console.ReadKey(true);
        }

        private async Task Register()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|----------------------------------------|");
            Console.WriteLine("|                Registration.           |");
            Console.WriteLine("|----------------------------------------|");
            Console.WriteLine();
            Console.ResetColor();

            string username;
            string password;

            Console.Write("Wählen Sie einen Benutzernamen: ");
            username = this.GetString();

            Console.Write("Wählen Sie ein Passwort: ");
            password = this.GetString();

            this.client = new SrpApplicationClient(username, password, new SRP_SDK.SrpClient(new SRP_SDK.SRPGroup()), new Uri("https://localhost:44314/"));

            try
            {
                await client.RegisterAsync($"api/user/getsalt/{username}", "api/user/registration/");
                Console.WriteLine($"Client mit den Daten {username} (Benutzername) und {password} (Passwort) hat sich registriert. Und kann sich ab sofort mit diesen einloggen.");
            }
            catch (Exception)
            {
                Console.WriteLine("Registration konnte nicht abgeschlossen werden. Dies liegt vermutlich daran, dass ein Benutzer sich bereits mit dem angegebenen Benutzernamen registriert hat.");
            }
            finally
            {
                Console.WriteLine("Bitte mit einem Tastendruck bestätigen");
                Console.ReadKey(true);
            }
        }

        private async Task StartSendMessages()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|-------------------------------------------|");
            Console.WriteLine("|Nachrichten übermitteln. Escape zum beenden|");
            Console.WriteLine("|-------------------------------------------|");
            Console.WriteLine();
            Console.ResetColor();

            if (this.currentSessionKey == null)
            {
                throw new InvalidOperationException("Kein Session key vorhanden. Zuerst einloggen bevor Nachrichten übermittelt werden können!");
            }

            try
            {
                bool isValidKey = await this.client.ComputeProof(this.currentSessionKey, "api/user/proof/postproof");

                if(isValidKey)
                    await this.SendMessages();
            }
            catch (CryptographicException)
            {
                throw new Exception("Schlüssel vorhanden, konnte aber nicht verifiziert werden. Bitte nochmal einloggen.");
            }
        }

        private async Task SendMessages()
        {
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                string clientMessage;
                string serverMessage;

                Console.Write("Enter message: ");
                clientMessage = Console.ReadLine();

                Console.WriteLine($"Plaintext to server sent: {clientMessage}");
                serverMessage = await this.client.SendMessage(this.currentSessionKey, clientMessage);
                Console.WriteLine($"Plaintext server received: {serverMessage}\n");
            }
        }

        private void DrawMenu()
        {
            Console.Clear();

            if (this.currentSessionKey != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Logged in as: {this.client.Username}");
                Console.WriteLine($"Current session key in base64 string format: {Convert.ToBase64String(this.currentSessionKey)}");
            }

            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < this.Menu.Entries.Count; i++)
            {
                Console.WriteLine($"{i}) {this.Menu.Entries[i].Name}");
            }

            Console.ResetColor();
        }

        private async Task<bool> ExecuteInput(ConsoleKey key)
        {
            try
            {
                switch (key)
                {
                    case ConsoleKey.D0:
                        await this.Menu.Entries[0].Action.Invoke();
                        break;
                    case ConsoleKey.D1:
                        await this.Menu.Entries[1].Action.Invoke();
                        break;
                    case ConsoleKey.D2:
                        await this.Menu.Entries[2].Action.Invoke();
                        break;
                    case ConsoleKey.D3:
                        this.isRunning = false;
                        break;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Der folgende Fehler ist aufgetreten: {e.Message}. bitte mit Tastendruck bestätigen!");
                Console.ReadKey(true);
            }

            return true;
        }

        private string GetString()
        {
            bool isValid;
            string input;

            do
            {
                input = Console.ReadLine();
                isValid = !string.IsNullOrWhiteSpace(input);
            }
            while (!isValid);

            return input;
        }
    }
}
