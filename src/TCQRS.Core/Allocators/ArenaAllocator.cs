// ini adalah alpha version
using System;
using System.Buffers;

namespace TCQRS.Core.Allocators
{
    /// <summary>
    /// Minimizes heap allocations by using ArrayPool for transient event buffers.
    /// </summary>
    public class ArenaAllocator<T> : IDisposable
    {
        private T[]? _buffer;
        private readonly int _capacity;

        public ArenaAllocator(int capacity)
        {
            _capacity = capacity;
            _buffer = ArrayPool<T>.Shared.Rent(capacity);
        }

        public Span<T> GetSpan()
        {
            if (_buffer == null) throw new ObjectDisposedException(nameof(ArenaAllocator<T>));
            return _buffer.AsSpan(0, _capacity);
        }

        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<T>.Shared.Return(_buffer);
                _buffer = null;
            }
        }
    }
}
