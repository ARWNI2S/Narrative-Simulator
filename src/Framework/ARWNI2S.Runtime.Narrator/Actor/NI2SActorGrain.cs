using ARWNI2S.Engine.Core;
using Orleans;

namespace ARWNI2S.Runtime.Narrator.Actor
{
    public class NI2SActorGrain : Grain<ActorState>, INiisGrain
    {
        public Task<ActorState> GetStateAsync()
        {
            return Task.FromResult(State);
        }

        public async Task PersistStateAsync()
        {
            await WriteStateAsync();
        }

        public Task UpdateStateAsync(string propertyName, object value)
        {
            State[propertyName] = value;
            return Task.CompletedTask;
        }

        public Task UpdateStateAsync(Dictionary<string, object> values)
        {
            foreach (var value in values)
            {
                State[value.Key] = value.Value;
            }
            return Task.CompletedTask;
        }

        public Task UpdateStateAsync(ActorState state)
        {
            State = state;
            return Task.CompletedTask;
        }
    }
}
