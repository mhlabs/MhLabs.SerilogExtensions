namespace MhLabs.SerilogExtensions
{
    public static class Constants
    {
        public const string OutputTemplateFormat = "[{Level:u3}] [{Properties:j}] {Message:lj}{Exception}{NewLine}";
        public const string CorrelationIdHeader = "mh-correlation-id";
        public const string RouteTemplateHeader = "routetemplate";
    }
}