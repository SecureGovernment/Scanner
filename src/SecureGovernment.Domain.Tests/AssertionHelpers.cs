using DnsClient;
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

        public static void AssertSpfReponse(ScanResult scanResult, List<string> records, bool hasSpfRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedSpfResponse));

            var spfReponse = scanResult as ParsedSpfResponse;

            CollectionAssert.AreEqual(records, spfReponse.Records);
            Assert.AreEqual(hasSpfRecords, spfReponse.HasSpfRecords);
            Assert.IsFalse(spfReponse.HasOldSpf);
            Assert.IsNull(spfReponse.OldSpfRecords);
        }

        public static void AssertMxReponse(ScanResult scanResult, List<string> records, bool hasMxRecords)
        {
            Assert.IsInstanceOfType(scanResult, typeof(ParsedMxResponse));

            var mxReponse = scanResult as ParsedMxResponse;

            CollectionAssert.AreEqual(records, mxReponse.Records);
            Assert.AreEqual(hasMxRecords, mxReponse.HasMxRecords);
        }

        public static void AssertPreviousScanResults(List<ScanResult> scanResults)
        {
            Assert.AreEqual(2, scanResults.Count);

            AssertSpfReponse(scanResults[0], new[] { "v=spf -all" }.ToList(), true);
            AssertMxReponse(scanResults[1], new[] { "aspmx.l.google.com.", "alt1.aspmx.l.google.com.", "alt2.aspmx.l.google.com." }.ToList(), true);
        }
    }
}
