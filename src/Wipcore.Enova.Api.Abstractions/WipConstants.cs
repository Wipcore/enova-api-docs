using System;

namespace Wipcore.Enova.Api.Abstractions
{
    public static class WipConstants
    {
        public static readonly DateTime DefaultStartAtDateTime = new DateTime(1980, 1, 1);

        public static readonly DateTime DefaultEndAtDateTime = new DateTime(2035, 12, 1);

        public static readonly DateTime InvalidDateTime = new DateTime(1980, 1, 1);

        public const string ElasticIndexHttpContextIdentifier = "elastic_context";
        public const string ElasticDeltaIndexHttpContextIdentifier = "elastic_delta_context";
        public const string InternalHttpContextIdentifier = "internal_context";

        public const string UserIdCookieIdentifier = "WebadminUserKey";

        public const string ContextModelKey = "requestContext";
        public const string EnovaContextKey = "enovaContext";

        public const bool ContinueOnCapturedContext = false;
    }
}
