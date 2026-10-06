using System;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class CloudCodeModuleRoutingTests
    {
        [Test]
        public void NewTransport_StartsWithNotRequestedDiagnostic()
        {
            UgsOnlineRecordTransport transport = (UgsOnlineRecordTransport)typeof(UgsOnlineRecordTransport)
                .GetConstructor(Type.EmptyTypes).Invoke(null);
            Assert.That(transport.LastDiagnostic, Is.EqualTo("ModulePhase=NotRequested"));
        }

        [TestCase("get-public-player-number", "GetPublicPlayerNumber")]
        [TestCase("get-account-transfer-status", "GetAccountTransferStatus")]
        [TestCase("get-account-personal-bests", "GetAccountPersonalBests")]
        [TestCase("start-account-transfer", "StartAccountTransfer")]
        [TestCase("reissue-account-transfer", "ReissueAccountTransfer")]
        [TestCase("cancel-account-transfer", "CancelAccountTransfer")]
        [TestCase("complete-account-transfer", "CompleteAccountTransfer")]
        [TestCase("submit-record", "SubmitRecord")]
        [TestCase("query-records", "QueryRecords")]
        public void Endpoint_MapsToVerificationModule(string endpoint, string function)
        {
            Assert.That(UgsOnlineRecordTransport.VerificationModuleName, Is.EqualTo("FlowStateVerification"));
            Assert.That(UgsOnlineRecordTransport.GetModuleFunctionName(endpoint), Is.EqualTo(function));
        }

        [TestCase("crypto")]
        [TestCase("OtherModule/StartAccountTransfer")]
        [TestCase("")]
        [TestCase(null)]
        public void UnknownEndpoint_IsRejected(string endpoint)
        {
            Assert.Throws<ArgumentException>(() => UgsOnlineRecordTransport.GetModuleFunctionName(endpoint));
        }
    }
}
