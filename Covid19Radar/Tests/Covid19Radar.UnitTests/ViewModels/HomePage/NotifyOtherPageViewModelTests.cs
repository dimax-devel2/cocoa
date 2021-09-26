﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class NotifyOtherPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<ICloseApplication> mockCloseApplication;
        private readonly Mock<IDiagnosisKeyRegisterServer> mockDiagnosisKeyRegisterServer;

        public NotifyOtherPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockCloseApplication = mockRepository.Create<ICloseApplication>();
            mockDiagnosisKeyRegisterServer = mockRepository.Create<IDiagnosisKeyRegisterServer>();
        }

        private NotifyOtherPageViewModel CreateViewModel()
        {
            return new NotifyOtherPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockCloseApplication.Object,
                new MockExposureNotificationApiService(mockLoggerService.Object),
                mockDiagnosisKeyRegisterServer.Object
                );
        }

        [Theory]
        [InlineData("11111111", false, false, false)]
        [InlineData("11111111", true, false, true)]
        [InlineData("11111111", false, true, true)]
        [InlineData("1111111", true, false, false)]
        [InlineData("111111111", true, false, false)]
        public void CheckRegisterButtonEnableTest(string processNumber, bool isVisibleWithSymptomsLayout, bool isVisibleNoSymptomsLayout, bool expectResult)
        {
            var vm = CreateViewModel();
            vm.ProcessNumber = processNumber;
            vm.IsVisibleWithSymptomsLayout = isVisibleWithSymptomsLayout;
            vm.IsVisibleNoSymptomsLayout = isVisibleNoSymptomsLayout;

            var result = vm.CheckRegisterButtonEnable();

            Assert.Equal(expectResult, result);
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    internal class MockExposureNotificationApiService : AbsExposureNotificationApiService
    {
        private const long DUMMY_EN_API_VERSION = 2;

        internal MockExposureNotificationApiService(
            ILoggerService loggerService
            ) : base(loggerService) {
        }

        public override async Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
            => new List<ExposureNotificationStatus>();

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
            => new List<TemporaryExposureKey>();

        public override async Task<long> GetVersionAsync()
            => DUMMY_EN_API_VERSION;

        public override async Task<bool> IsEnabledAsync()
            => true;

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles)
        {
            // do nothing
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration)
        {
            // do nothing
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, string token)
        {
            // do nothing
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            // do nothing
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            // do nothing
        }

        public override async Task StartAsync()
        {
            // do nothing
        }

        public override async Task StopAsync()
        {
            // do nothing
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}