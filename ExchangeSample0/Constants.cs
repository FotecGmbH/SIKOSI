// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 07:43
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using LibGit2Sharp;

namespace SIKOSI.Exchange
{
    public class Constants
    {
        private static readonly Lazy<Constants> lazy = new Lazy<Constants>(() => new Constants());

        /// <summary>
        ///     Infor about the repo
        /// </summary>
        public string RepositoryVersionInfo = "";

        /// <summary>
        ///     SELECT the sample you want to test
        /// </summary>
        public SampleEnum Sample = SampleEnum.Sample_0;

        /// <summary>
        ///     Description what the Sample do
        /// </summary>
        public string SampleDescription = "";

        private Constants()
        {
            using (var repo = new Repository())
            {
                var currentBranchName = repo.Head.FriendlyName;
            }

            switch (Sample)
            {
                case SampleEnum.Sample_0:
                    ApiUrl = "https://localhost:44360/";
                    SampleDescription = "Sample 0 - Basis für weiter Samples";
                    break;
            }
        }

        #region Properties

        /// <summary>
        ///     URL of the API
        /// </summary>
        public static string ApiUrl { get; set; }

        public static Constants Instance
        {
            get { return lazy.Value; }
        }

        #endregion
    }
}