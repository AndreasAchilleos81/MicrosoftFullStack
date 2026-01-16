namespace HashTables
{
    public class ConcurrentHashTable<Tkey, TValue> where Tkey : class
    {
        private const int INITIAL_SIZE = 16;
        private LinkedList<LinkedListNode<KeyValuePair<Tkey, TValue>>>[] buckets;
        private readonly ReaderWriterLockSlim[] locks;
        private int count;
        private int capacity;

        public ConcurrentHashTable(int intitialCapacity = INITIAL_SIZE)
        {
            capacity = intitialCapacity;
            buckets = new LinkedList<LinkedListNode<KeyValuePair<Tkey, TValue>>>[capacity];
            locks = new ReaderWriterLockSlim[capacity];

            for (int i = 0; i < capacity; i++)
            {
                buckets[i] = new LinkedList<LinkedListNode<KeyValuePair<Tkey, TValue>>>();
                locks[i] = new ReaderWriterLockSlim();
            }
        }

        private int GetBucketIndex(Tkey key)
        {
            return Math.Abs(key.GetHashCode()) % capacity;
        }

        public void Add(Tkey key, TValue value)
        {
            int index = GetBucketIndex(key);
            locks[index].EnterWriteLock();

            try
            {
                var bucket = buckets[index];

                foreach (var pair in bucket)
                {
                    if (pair.Value.Key.Equals(key))
                    {
                        throw new InvalidOperationException($"key '{key}' already exists,");
                    }
                }
                
                var keyValuePair = new KeyValuePair<Tkey, TValue>(key, value);  

                bucket.AddLast(new LinkedListNode<KeyValuePair<Tkey, TValue>>(keyValuePair));
                Interlocked.Increment(ref count);
            }
            finally
            {
                locks[index].ExitWriteLock();
            }
        }

        public bool TryGetValue(Tkey key, out TValue value)
        {
            int index = GetBucketIndex(key);
            locks[index].EnterReadLock();

            try
            {
                var bucket = buckets[index];
                foreach (var pair in bucket)
                {
                    if (pair.Value.Key.Equals(key))
                    {
                        value = pair.Value.Value;
                        return true;
                    }
                }

                value = default(TValue);
            }
            finally
            {
                locks[index].ExitReadLock();
            }

            return false;
        }

        public bool Remove(Tkey key)
        {
            int index = GetBucketIndex(key);
            locks[index].EnterWriteLock();

            try
            {
                var bucket = buckets[index];
                var node = bucket.First();
                while(node.Value.Key != default(Tkey))
                {
                    if (node.Value.Key == key)
                    {
                        bucket.Remove(node);
                        return true;
                    }
                    node = node.Next;
                }

                return false;
            }
            finally
            {
                locks[index].ExitWriteLock();
            }

            return false;
        }

        public IEnumerable<Tkey> Keys
        {
            get
            {
                foreach (var bucket in buckets)
                {
                    foreach (var pair in bucket)
                    {
                        yield return pair.Value.Key;
                    }
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var bucket in buckets)
                {
                    foreach (var pair in bucket)
                    {
                        yield return pair.Value.Value;
                    }
                }
            }
        }

        private void Resize()
        {
            int newCapacity = capacity * 2;
            var newBuckets = new LinkedList<LinkedListNode<KeyValuePair<Tkey, TValue>>>[newCapacity];

            for (int i = 0; i < newCapacity; i++)
            {
                newBuckets[i] = new LinkedList<LinkedListNode<KeyValuePair<Tkey, TValue>>>();
            }

            // Rehash all existing entries
            foreach (var bucket in buckets)
            {
                foreach (var pair in bucket)
                {
                    int newIndex = GetBucketIndex(pair.Value.Key);
                    newBuckets[newIndex].AddLast(pair);
                }
            }

            buckets = newBuckets;
            capacity = newCapacity;
        }
    }
}
