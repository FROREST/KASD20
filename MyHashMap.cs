using System;
using System.Collections.Generic;

public class MyHashMap<K, V>
{
    private Entry[] table; // Массив для хранения пар "ключ-значение"
    private int size; // Количество пар "ключ-значение"
    private float loadFactor; // Коэффициент загрузки
    private int threshold; // Порог, при котором размер массива увеличивается

    // Вложенный класс для представления пары "ключ-значение"
    private class Entry
    {
        public K Key;
        public V Value;
        public Entry Next;

        public Entry(K key, V value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    // Конструктор по умолчанию с начальной ёмкостью 16 и коэффициентом загрузки 0.75
    public MyHashMap() : this(16, 0.75f) { }

    // Конструктор с указанной начальной ёмкостью и коэффициентом загрузки 0.75
    public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75f) { }

    // Конструктор с указанной начальной ёмкостью и коэффициентом загрузки
    public MyHashMap(int initialCapacity, float loadFactor)
    {
        if (initialCapacity <= 0)
            throw new ArgumentException("Initial capacity must be greater than zero.");
        if (loadFactor <= 0 || float.IsNaN(loadFactor))
            throw new ArgumentException("Load factor must be greater than zero.");

        this.loadFactor = loadFactor;
        table = new Entry[initialCapacity];
        threshold = (int)(initialCapacity * loadFactor);
        size = 0;
    }

    // Очистить все элементы в отображении
    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
        size = 0;
    }

    // Проверка наличия ключа
    public bool ContainsKey(object key)
    {
        int index = GetBucketIndex(key);
        Entry entry = table[index];
        while (entry != null)
        {
            if (Equals(entry.Key, key)) return true;
            entry = entry.Next;
        }
        return false;
    }

    // Проверка наличия значения
    public bool ContainsValue(object value)
    {
        foreach (var entry in table)
        {
            Entry current = entry;
            while (current != null)
            {
                if (Equals(current.Value, value)) return true;
                current = current.Next;
            }
        }
        return false;
    }

    // Возвращает множество всех пар "ключ-значение"
    public HashSet<KeyValuePair<K, V>> EntrySet()
    {
        var entrySet = new HashSet<KeyValuePair<K, V>>();
        foreach (var entry in table)
        {
            Entry current = entry;
            while (current != null)
            {
                entrySet.Add(new KeyValuePair<K, V>(current.Key, current.Value));
                current = current.Next;
            }
        }
        return entrySet;
    }

    // Получить значение по ключу
    public V Get(object key)
    {
        int index = GetBucketIndex(key);
        Entry entry = table[index];
        while (entry != null)
        {
            if (Equals(entry.Key, key)) return entry.Value;
            entry = entry.Next;
        }
        return default(V);
    }

    // Проверка на пустоту
    public bool IsEmpty()
    {
        return size == 0;
    }

    // Возвращает множество всех ключей
    public HashSet<K> KeySet()
    {
        var keySet = new HashSet<K>();
        foreach (var entry in table)
        {
            Entry current = entry;
            while (current != null)
            {
                keySet.Add(current.Key);
                current = current.Next;
            }
        }
        return keySet;
    }

    // Добавить пару "ключ-значение"
    public void Put(K key, V value)
    {
        int index = GetBucketIndex(key);
        Entry entry = table[index];

        while (entry != null)
        {
            if (Equals(entry.Key, key))
            {
                entry.Value = value; // Обновление значения, если ключ уже существует
                return;
            }
            entry = entry.Next;
        }

        // Добавление новой пары
        Entry newEntry = new Entry(key, value);
        newEntry.Next = table[index];
        table[index] = newEntry;
        size++;

        // Проверка, нужно ли увеличить ёмкость
        if (size >= threshold)
        {
            Resize();
        }
    }

    // Удалить пару по ключу
    public bool Remove(object key)
    {
        int index = GetBucketIndex(key);
        Entry entry = table[index];
        Entry prev = null;

        while (entry != null)
        {
            if (Equals(entry.Key, key))
            {
                if (prev != null)
                {
                    prev.Next = entry.Next;
                }
                else
                {
                    table[index] = entry.Next;
                }
                size--;
                return true;
            }
            prev = entry;
            entry = entry.Next;
        }
        return false;
    }

    // Получить размер отображения
    public int Size()
    {
        return size;
    }

    // Вспомогательная функция для получения индекса бакета
    private int GetBucketIndex(object key)
    {
        return Math.Abs(key.GetHashCode()) % table.Length;
    }

    // Функция для увеличения ёмкости хэш-таблицы
    private void Resize()
    {
        int newCapacity = table.Length * 2;
        Entry[] newTable = new Entry[newCapacity];
        foreach (var entry in table)
        {
            Entry current = entry;
            while (current != null)
            {
                int newIndex = Math.Abs(current.Key.GetHashCode()) % newCapacity;
                Entry next = current.Next;
                current.Next = newTable[newIndex];
                newTable[newIndex] = current;
                current = next;
            }
        }
        table = newTable;
        threshold = (int)(newCapacity * loadFactor);
    }
}