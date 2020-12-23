// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 13:14
// Developer      Benjamin Moser
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

namespace SIKOSI.SampleDatabase02.Entities
{
    public class TblMessage
    {
        #region Properties

        /// <summary>
        ///     Id der Message
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Sender ID der Message
        /// </summary>
        public int SenderID { get; set; }

        /// <summary>
        ///     Empfänger Id der Message
        /// </summary>
        public int ReceiverId { get; set; }

        /// <summary>
        ///     Ob Gruppen-Nachricht (true) oder persönliche Nachricht (false)
        /// </summary>
        public bool IsGroupMessage { get; set; }

        /// <summary>
        ///     Verschlüsseltes Message Array
        /// </summary>
        public byte[] Message { get; set; }

        #endregion
    }
}