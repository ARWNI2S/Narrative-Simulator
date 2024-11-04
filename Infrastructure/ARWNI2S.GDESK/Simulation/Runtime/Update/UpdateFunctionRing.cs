using ARWNI2S.Infrastructure.Collections.Generic;
using System.Collections;

namespace ARWNI2S.Engine.Simulation.Runtime.Update
{
    internal class UpdateFunctionRing : IEnumerable<UpdateFunction>, IEnumerator<UpdateFunction>
    {
        private UpdateFrameRoot updateFrameRoot;

        private UpdateFunction current;

        private Deque<UpdateRoot> _completedCycles = [];
        private UpdateRoot currentCycleRoot;
        private Deque<UpdateRoot> _pendingCycles = [];

        private bool _inSegment;
        private bool _isFrameComplete;

        public void Initialize(UpdateFrameRoot frameRoot)
        {
            updateFrameRoot = frameRoot ?? throw new ArgumentNullException(nameof(frameRoot));
            current = updateFrameRoot;
            Reset();
        }

        public UpdateFunction GetNextFrameRoot() => _isFrameComplete ? PrepareFrameRoot() : updateFrameRoot;

        private UpdateFrameRoot PrepareFrameRoot()
        {
            return updateFrameRoot;//TODO: .GenerateNextFrame();
        }

        public UpdateFunction GetNextCycleRoot()
        {
            if (current == null)
                return updateFrameRoot;

            var result = current.InternalData.Next;

            while (result is not UpdateRoot)
                result = result.InternalData.Next;

            return result;
        }

        #region IEnumerable implementation

        public IEnumerator<UpdateFunction> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerator implementation

        public UpdateFunction Current => current;

        object IEnumerator.Current => Current;

        // Mueve al siguiente UpdateFunction en el segmento actual
        public bool MoveNext()
        {
            if (current == null) // Primer acceso al segmento actual
            {
                current = currentCycleRoot;
            }
            else
            {
                current = current.InternalData.Next;
            }

            //if(current is UpdateFrameRoot frameRoot)
            //{
            //    currentCycleRoot = updateFrameRoot = frameRoot;
            //    _inSegment = false;
            //}
            // Si llegamos al próximo UpdateRoot, hemos alcanzado el final del segmento actual
            if (current is UpdateRoot nextRoot && current != currentCycleRoot)
            {
                currentCycleRoot = nextRoot;
                _inSegment = false;
            }

            return _inSegment;
        }

        public void Reset()
        {
            currentCycleRoot = (UpdateRoot)current; // Mantener la posición en el anillo general
            current = null;
            _inSegment = true;
        }

        public void Dispose() { /* No cleanup necessary */ }

        #endregion
    }
}

