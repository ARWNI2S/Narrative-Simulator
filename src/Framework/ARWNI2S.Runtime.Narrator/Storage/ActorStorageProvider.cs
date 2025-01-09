using Orleans.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARWNI2S.Runtime.Narrator.Storage
{
    internal class ActorStorageProvider : IStorage<ActorState>
    {
        public ActorState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Etag => throw new NotImplementedException();

        public bool RecordExists => throw new NotImplementedException();

        public Task ClearStateAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReadStateAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteStateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
