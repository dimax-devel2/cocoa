﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Covid19Radar.Model;

namespace Covid19Radar.Common
{
    public static class DeviceVerifierUtils
    {
        public static string GetNonceClearTextV2(DiagnosisSubmissionParameter submission)
        {
            return string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);

            static string GetKeyString(IEnumerable<DiagnosisSubmissionParameter.Key> keys) =>
                string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyStringCore(k)));

            static string GetKeyStringCore(DiagnosisSubmissionParameter.Key k) =>
                string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod);

            static string GetRegionString(IEnumerable<string> regions) =>
                string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
        }

        public static byte[] CreateAndroidNonceV2(DiagnosisSubmissionParameter submission)
        {
            var cleartext = GetNonceClearTextV2(submission);
            var nonce = GetSha256(cleartext);
            return nonce;

            static byte[] GetSha256(string text)
            {
                using var sha = SHA256.Create();
                var textBytes = Encoding.UTF8.GetBytes(text);
                return sha.ComputeHash(textBytes);
            }
        }
    }
}
