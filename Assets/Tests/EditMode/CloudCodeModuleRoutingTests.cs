using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public sealed class CloudCodeModuleRoutingTests
    {
        [TestCase(1)]
        [TestCase(99)]
        public void LegacySubmitHelper_StagePayloadRejectsBeforePermissionOrTransport(int version)
        {
            Type type = Type.GetType("FlowState.Editor.OnlineRecordVerificationWindow, Assembly-CSharp-Editor");
            Assert.That(type, Is.Not.Null);
            MethodInfo method = type.GetMethod("CallSubmitAsync", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            OnlineRecordRequest payload = new OnlineRecordRequest { boardId = "fs-stage-stage-001-r1", rulesVersion = version };
            Task<OnlineSubmissionResponse> work = (Task<OnlineSubmissionResponse>)method.Invoke(null, new object[] { null, payload });
            NotSupportedException error = Assert.ThrowsAsync<NotSupportedException>(async () => await work);
            Assert.That(error.Message, Is.EqualTo("StageModeUnsupported"));
        }

        [TestCase("submit-record", false)]
        [TestCase("submit-record", true)]
        [TestCase("query-records", false)]
        [TestCase("query-records", true)]
        public void RawStageRequest_IsRejectedBeforeSdkOrDiagnosticMutation(string endpoint, bool dictionary)
        {
            UgsOnlineRecordTransport transport = CreateTransport();
            const string payload = "{\"boardId\":\"fs-stage-stage-001-r1\"}";
            NotSupportedException error = Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                if (dictionary) await transport.CallAsync<object>(endpoint, new Dictionary<string, string> { { "request", payload } });
                else await transport.CallAsync<object>(endpoint, payload);
            });
            Assert.That(error.Message, Is.EqualTo("StageModeUnsupported"));
            Assert.That(transport.LastDiagnostic, Is.EqualTo("ModulePhase=NotRequested"));
            Assert.That(typeof(UgsOnlineRecordTransport).GetField("_requestSequence",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(transport), Is.EqualTo(0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("{")]
        [TestCase("null")]
        [TestCase("[]")]
        [TestCase("{}")]
        [TestCase("{\"boardId\":\"unknown\"}")]
        [TestCase("{\"boardId\":\"fs-infinite-v2\",\"kind\":\"other\",\"limit\":10}")]
        public void RawInvalidQuery_IsRejectedBeforeSdk(string payload)
        {
            UgsOnlineRecordTransport transport = CreateTransport();
            Assert.ThrowsAsync<ArgumentException>(async () => await transport.CallAsync<object>("query-records", payload));
            Assert.That(transport.LastDiagnostic, Is.EqualTo("ModulePhase=NotRequested"));
        }

        [Test]
        public void RawRecordDictionaryWithoutRequest_IsRejectedBeforeSdk()
        {
            UgsOnlineRecordTransport transport = CreateTransport();
            Assert.ThrowsAsync<ArgumentException>(async () => await transport.CallAsync<object>("query-records",
                new Dictionary<string, string>()));
            Assert.That(transport.LastDiagnostic, Is.EqualTo("ModulePhase=NotRequested"));
        }

        [Test]
        public void RawSubmissionWithOldVersion_IsRejectedBeforeSdk()
        {
            UgsOnlineRecordTransport transport = CreateTransport();
            Assert.ThrowsAsync<ArgumentException>(async () => await transport.CallAsync<object>("submit-record",
                "{\"boardId\":\"fs-infinite-v2\",\"rulesVersion\":1}"));
            Assert.That(transport.LastDiagnostic, Is.EqualTo("ModulePhase=NotRequested"));
        }

        [TestCase("submit-record")]
        [TestCase("query-records")]
        public void InfiniteRawValidation_UsesExistingBoardAndEndpointContract(string endpoint)
        {
            string payload = UnityEngine.JsonUtility.ToJson(new OnlineRecordRequest
            {
                boardId = "fs-infinite-v2", rulesVersion = ScoringVersion.Current,
                submissionId = "00000000-0000-4000-8000-000000000001", score = 60,
                runDurationMilliseconds = 60000, baseDistanceScore = 60, distanceScore = 60,
                totalScore = 60, maximumMomentumMultiplier = 1, kind = "me", limit = 1
            });
            Assert.DoesNotThrow(() => OnlineRecordConfiguration.ValidateRecordRequest(endpoint, payload));
        }

        [Test]
        public void AccountSnapshotEndpoint_DoesNotApplyUserRecordBoardFilter()
        {
            Assert.DoesNotThrow(() => OnlineRecordConfiguration.ValidateRecordRequest("get-account-personal-bests",
                "{\"boardId\":\"fs-stage-stage-001-r1\"}"));
        }

        private static UgsOnlineRecordTransport CreateTransport()
        {
            return (UgsOnlineRecordTransport)typeof(UgsOnlineRecordTransport)
                .GetConstructor(Type.EmptyTypes).Invoke(null);
        }

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
