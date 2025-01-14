﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using AndroidX.Work;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Droid.Services.Migration
{
    internal class WorkManagerMigrator
    {
        // Array of old work-name.
        private static readonly string[] OldWorkNames = {
            "exposurenotification",
            "cocoaexposurenotification",
        };

        private readonly ILoggerService _loggerService;

        public WorkManagerMigrator(
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
        }

        internal Task ExecuteAsync()
        {
            _loggerService.StartMethod();

            var workManager = WorkManager.GetInstance(Xamarin.Essentials.Platform.AppContext);
            CancelOldWorks(workManager, OldWorkNames, _loggerService);

            Xamarin.ExposureNotifications.ExposureNotification.Init();

            _loggerService.EndMethod();

            return Task.CompletedTask;
        }

        private static void CancelOldWorks(WorkManager workManager, string[] oldWorkNames, ILoggerService loggerService)
        {
            loggerService.StartMethod();

            foreach (var oldWorkName in oldWorkNames)
            {
                workManager.CancelUniqueWork(oldWorkName);
                loggerService.Info($"Worker {oldWorkName} is canceled.");
            }

            loggerService.EndMethod();
        }
    }
}
