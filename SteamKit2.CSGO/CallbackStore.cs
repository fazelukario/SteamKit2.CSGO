using System;
using System.Collections.Generic;
using SteamKit2.GC;

namespace SteamKit2.CSGO
{
    /// <summary>
    ///     Stores all callbacks based on time it was places, first created -> first executed
    /// </summary>
    internal class CallbackStore
    {
        private readonly Dictionary<uint, Queue<Action<IPacketGCMsg>>> _dict = new Dictionary<uint, Queue<Action<IPacketGCMsg>>>();

        public bool TryGetValue(uint key, out Action<IPacketGCMsg> func)
        {
            if (_dict.ContainsKey(key) && (_dict[key].Count != 0))
            {
                func = _dict[key].Dequeue();
                return true;
            }
            func = null;
            return false;
        }

        public void Add(uint key, Action<IPacketGCMsg> action)
        {
            if (!_dict.ContainsKey(key))
                _dict.Add(key, new Queue<Action<IPacketGCMsg>>());

            _dict[key].Enqueue(action);
        }
    }
}