using System.Collections;

namespace ARWNI2S.Engine.Simulation.Runtime.Update
{
    internal class UpdateFunctionRing : IEnumerable<UpdateFunction>, IEnumerator<UpdateFunction>
    {
        private readonly UpdateFrameRoot _frameRoot;

        private UpdateFunction _current;
        private UpdateFunction _currentSegmentRoot;
        private bool _inSegment;
        private bool _isFrameComplete;

        public UpdateFunctionRing(UpdateFrameRoot frameRoot)
        {
            _frameRoot = frameRoot ?? throw new ArgumentNullException(nameof(frameRoot));
            _current = _currentSegmentRoot = _frameRoot;
            Reset();
        }

        public UpdateFunction GetNextFrameRoot() => _isFrameComplete ? PrepareFrameRoot() : _frameRoot;

        private UpdateFrameRoot PrepareFrameRoot()
        {
            return _frameRoot;//TODO: .GenerateNextFrame();
        }

        public UpdateFunction GetNextCycleRoot()
        {
            var result = _current.InternalData.Next;

            while (result is not UpdateRoot)
                result = result.InternalData.Next;

            return result;
        }

        #region IEnumerable implementation

        public IEnumerator<UpdateFunction> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerator implementation

        public UpdateFunction Current => _current;

        object IEnumerator.Current => Current;

        // Mueve al siguiente UpdateFunction en el segmento actual
        public bool MoveNext()
        {
            if (_current == null) // Primer acceso al segmento actual
            {
                _current = _currentSegmentRoot;
            }
            else
            {
                _current = _current.InternalData.Next;
            }

            // Si llegamos al próximo UpdateRoot, hemos alcanzado el final del segmento actual
            if (_current is UpdateRoot && _current != _currentSegmentRoot)
            {
                _currentSegmentRoot = _current;
                _inSegment = false;
            }

            return _inSegment;
        }

        public void Reset()
        {
            _currentSegmentRoot = _current; // Mantener la posición en el anillo general
            _current = null;
            _inSegment = true;
        }

        public void Dispose() { /* No cleanup necessary */ }

        #endregion
    }
}

