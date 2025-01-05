using ARWNI2S.Engine.Core;
using ARWNI2S.Engine.Core.Object;
using System.Collections;

namespace ARWNI2S.Framework.Character.Automation
{
    public class AiTeamController : NI2SObject, IController, IEnumerable<AiCharacter>
    {
        private Enumerator enumerator;

        public List<AiCharacter> TeamMembers { get; } = [];
        public List<AiCharacterController> TeamControllers { get; } = [];

        internal void AssumeControl(AiCharacter actor)
        {
            var inner = New<AiTeamMemberController>();
            inner.Owner = this;
            inner.AssumeControl(actor);
            TeamMembers.Add(actor);
            TeamControllers.Add(inner);
        }

        private void DropControl(AiCharacter actor)
        {
            if (TeamMembers.Contains(actor))
            {
                var index = TeamMembers.IndexOf(actor);
                TeamMembers.RemoveAt(index);
            }
        }

        private void DropControl(AiTeamMemberController teamController)
        {
            if (TeamControllers.Contains(teamController))
            {
                DropControl(teamController.AiCharacter);
                var index = TeamControllers.IndexOf(teamController);
                TeamControllers.RemoveAt(index);
            }
        }

        public INiisActor ControlledActor => enumerator.Current;

        void IController.AssumeControl(INiisActor actor) => AssumeControl((AiCharacter)actor);

        public IEnumerator<AiCharacter> GetEnumerator() { enumerator = new(TeamMembers); return enumerator; }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IController.DropControl() => throw new InvalidOperationException($"Cannot call {nameof(DropControl)} in a {nameof(AiTeamController)}");

        private class Enumerator : IEnumerator<AiCharacter>
        {
            private int _index = -1;
            private readonly IList<AiCharacter> _list;

            public AiCharacter Current => _list[_index];
            object IEnumerator.Current => Current;

            public Enumerator(IList<AiCharacter> list)
            {
                _list = list;
            }

            public void Dispose() { }
            public bool MoveNext()
            {
                return ++_index < _list.Count;
            }
            public void Reset()
            {
                _index = -1;
            }
        }

        private class AiTeamMemberController : AiCharacterController
        {
            public AiTeamController Owner { get; set; }

            internal override void DropControl()
            {
                Owner.DropControl(this);
                base.DropControl();
            }
        }

    }
}