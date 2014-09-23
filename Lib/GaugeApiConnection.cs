﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.ProtocolBuffers;
using main;

namespace Gauge.CSharp.Lib
{
    public class GaugeApiConnection : AbstractGaugeConnection
    {
        public GaugeApiConnection(int port) : base(port)
        {
        }
        public IEnumerable<string> GetStepValue(IEnumerable<string> stepTexts, bool hasInlineTable)
        {
            foreach (var stepText in stepTexts)
            {
                var stepValueRequest = GetStepValueRequest.CreateBuilder()
                    .SetStepText(stepText)
                    .SetHasInlineTable(hasInlineTable)
                    .Build();
                var stepValueRequestMessage = APIMessage.CreateBuilder()
                    .SetMessageId(GenerateMessageId())
                    .SetMessageType(APIMessage.Types.APIMessageType.GetStepValueRequest)
                    .SetStepValueRequest(stepValueRequest)
                    .Build();
                var apiMessage = WriteAndReadApiMessage(stepValueRequestMessage);
                yield return apiMessage.StepValueResponse.StepValue.StepValue;
            }
        }
        private static APIMessage ReadMessage(Stream networkStream)
        {
            var responseBytes = ReadBytes(networkStream);
            return APIMessage.ParseFrom(responseBytes.ToArray());
        }

        private APIMessage WriteAndReadApiMessage(IMessageLite stepValueRequestMessage)
        {
            lock (TcpCilent)
            {
                WriteMessage(stepValueRequestMessage);
                return ReadMessage(TcpCilent.GetStream());
            }
        }
    }
}