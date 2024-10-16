namespace ARWNI2S.Portal.Framework.Routing
{
    /// <summary>
    /// Represents route publisher
    /// </summary>
    public partial interface IRoutePublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder);
    }
}
