/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Android.Gms.SafetyNet;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;

namespace Covid19Radar.Droid.Services
{
    public class DeviceCheckService : IDeviceVerifier
    {
        public Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
        {
            var nonce = DeviceVerifierUtils.CreateAndroidNonceV2(submission);
            return GetSafetyNetAttestationAsync(nonce);
        }

        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
        private static async Task<string> GetSafetyNetAttestationAsync(byte[] nonce)
        {
            using var client = SafetyNetClass.GetClient(Android.App.Application.Context);
            using var response = await client.AttestAsync(nonce, AppSettings.Instance.AndroidSafetyNetApiKey);
            return response.JwsResult;
        }
    }
}
