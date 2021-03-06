﻿using DnsClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Tests
{
    public static class AssertionHelpers
    {
        public static void AssertDkimResponse(ScanResult scanResult, List<string> records, List<(string, QueryType, List<string>)> dkimRecords, bool hasDkimRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedDkimResponse));

            var dkimReponse = scanResult as ParsedDkimResponse;
            CollectionAssert.AreEqual(records, dkimReponse.Records);
            Assert.AreEqual(dkimRecords.Count, dkimReponse.DkimRecords.Count);

            for (int i = dkimReponse.DkimRecords.Count - 1; i >= 0; i--)
            {
                Assert.AreEqual(dkimRecords[i].Item1, dkimReponse.DkimRecords[i].Selector);
                Assert.AreEqual(dkimRecords[i].Item2, dkimReponse.DkimRecords[i].Item2);
                CollectionAssert.AreEqual(dkimRecords[i].Item3, dkimReponse.DkimRecords[i].Reponse);
            }

            Assert.AreEqual(hasDkimRecords, dkimReponse.HasDkim);
        }

        public static void AssertMxResponse(ScanResult scanResult, List<string> records, bool hasMxRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedMxResponse));

            var mxReponse = scanResult as ParsedMxResponse;

            CollectionAssert.AreEqual(records, mxReponse.Records);
            Assert.AreEqual(hasMxRecords, mxReponse.HasMxRecords);
        }

        public static void AssertDmarcResponse(ScanResult scanResult, List<string> records, bool hasDmarcRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedDmarcResponse));

            var dmarcResponse = scanResult as ParsedDmarcResponse;

            CollectionAssert.AreEqual(records, dmarcResponse.Records);
            Assert.AreEqual(hasDmarcRecords, dmarcResponse.HasDmarcRecords);
        }

        public static void AssertDnssecResponse(ScanResult scanResult, List<string> records, bool hasDnssecRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedDnssecResponse));

            var dnssecResponse = scanResult as ParsedDnssecResponse;

            CollectionAssert.AreEqual(records, dnssecResponse.Records);
            Assert.AreEqual(hasDnssecRecords, dnssecResponse.HasDnssec);
        }

        public static void AssertSpfResponse(ScanResult scanResult, List<string> records, List<string> txtTypeRecords, List<string> spfTypeRecords, bool hasTxtTypeRecords, bool hasSpfTypeRecords, bool hasRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedSpfResponse));

            var spfResponse = scanResult as ParsedSpfResponse;

            CollectionAssert.AreEqual(records, spfResponse.Records);
            CollectionAssert.AreEqual(txtTypeRecords, spfResponse.TxtTypeSpfRecords);
            CollectionAssert.AreEqual(spfTypeRecords, spfResponse.SpfTypeSpfRecords);
            Assert.AreEqual(hasSpfTypeRecords, spfResponse.HasSpfTypeRecords);
            Assert.AreEqual(hasTxtTypeRecords, spfResponse.HasTxtTypeRecords);
            Assert.AreEqual(hasRecords, spfResponse.HasSpfRecords);
        }


        public static void AssertPreviousScanResults(List<ScanResult> scanResults)
        {
            Assert.AreEqual(2, scanResults.Count);

            AssertDnssecResponse(scanResults[0], new[] { "DNSSEC KEY" }.ToList(), true);
            AssertMxResponse(scanResults[1], new[] { "aspmx.l.google.com.", "alt1.aspmx.l.google.com.", "alt2.aspmx.l.google.com." }.ToList(), true);
        }
    }
}
