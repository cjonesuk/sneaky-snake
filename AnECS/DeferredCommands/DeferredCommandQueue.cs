using System.Runtime.CompilerServices;

namespace AnECS;


unsafe delegate void DeferredCommandAction(World world, void* payload);


internal sealed class DeferredCommandQueue
{
    private readonly List<DeferredCommand> _commands;
    private byte[] _payload;
    private int _used;

    public DeferredCommandQueue()
    {
        _commands = new List<DeferredCommand>();
        _payload = new byte[1024];
        _used = 0;
    }

    private void EnsureCapacity(int bytes)
    {
        int needed = _used + bytes;
        if (needed > _payload.Length)
        {
            int newSize = _payload.Length;
            while (newSize < needed)
                newSize *= 2;
            Array.Resize(ref _payload, newSize);
        }
    }

    public unsafe void Write<T>(ref T value, DeferredCommandAction action)
        where T : unmanaged
    {
        int size = sizeof(T);
        EnsureCapacity(size);

        fixed (byte* pBase = _payload)
        {
            void* dest = pBase + _used;
            Buffer.MemoryCopy(Unsafe.AsPointer(ref value), dest, size, size);
            _commands.Add(new DeferredCommand(action, _used));
        }

        _used += size;
    }

    public unsafe void ApplyAll(World world)
    {
        if (_commands.Count == 0)
            return;

        // Pin the managed array
        fixed (byte* pBase = _payload)
        {
            foreach (var cmd in _commands)
            {
                // Compute pointer into the fixed buffer
                void* payloadPtr = pBase + cmd.PayloadOffset;

                // Execute the recorded action
                cmd.Apply(world, payloadPtr);
            }
        }

        _commands.Clear();
        _used = 0;
    }
}
