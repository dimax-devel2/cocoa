﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SendLogCompletePageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        private string _LogId;

        public Func<string, string, string[], Task> ComposeEmailAsync { get; set; } = Email.ComposeAsync;

        public Func<string, Task> CopyIdAsync { get; set; } = Clipboard.SetTextAsync;

        public string LogId
        {
            get { return _LogId; }
            set { SetProperty(ref _LogId, value); }
        }

        public SendLogCompletePageViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            this.loggerService = loggerService;
        }

        public Command OnClickSendMailCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            try
            {
                // Send mail with log
                await ComposeEmailAsync(
                    Resources.AppResources.SendIdMailSubject,
                    Resources.AppResources.SendIdMailBody1 + LogId + Resources.AppResources.SendIdMailBody2.Replace("\\r\\n", "\r\n"),
                    new string[] { AppSettings.Instance.SupportEmail });

                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                loggerService.Exception("Exception", ex);
                loggerService.EndMethod();
            }
        });

        public Command OnCopyCommand => new Command(async () =>
        {
            try
            {
                await CopyIdAsync(LogId);
                await UserDialogs.Instance.AlertAsync(
                    null,
                    title: Resources.AppResources.SuccessMessageToCopyLogId,
                    okText: Resources.AppResources.ButtonOk);
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to copy operationing information ID.", ex);
            }
        });

        public Command OnClickHomeCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            await NavigateToHomeAsync();
            loggerService.EndMethod();
        });

        public Command OnBackButtonPressedCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            await NavigateToHomeAsync();
            loggerService.EndMethod();
        });

        public override void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();

            base.Initialize(parameters);
            LogId = parameters["logId"] as string;

            loggerService.EndMethod();
        }

        private async Task NavigateToHomeAsync()
        {
            _ = await NavigationService.NavigateAsync($"/{nameof(MenuPage)}/{nameof(NavigationPage)}/{nameof(HomePage)}");
        }
    }
}
