﻿using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Engine.Builder;

namespace ARWNI2S.Connector.Infrastructure
{
    internal class ConnectorStartup : INodeStartup
    {
        public int Order => 2002;

        public void Configure(IEngineBuilder engine)
        {

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
