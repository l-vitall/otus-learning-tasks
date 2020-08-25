using System;
using Calories.Common;
using OpenIddict.Abstractions;

namespace Calories.Records
{
    public class GrpcEnvironmentSettings : EnvironmentSettings
    {
        public string GrpcUserServer { get; set; }
        public string Database2Uri { get; set; }

        public GrpcEnvironmentSettings()
        {
            GrpcUserServer = Environment.GetEnvironmentVariable("GRPC_USER_SERVER_URI");
            Database2Uri = Environment.GetEnvironmentVariable("DATABASE2_URI");
        }
    }
}