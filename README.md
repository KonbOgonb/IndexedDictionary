# IndexedDictionary
Dictionary with the ability to add custom SQL-Like index. Index is passed like delegate projection from TKey, TValue.
Can be useful when needed access to dictionary data not through its' key but through some other properties/projections of stored data.
Instead of iterations through all KeyValue pairs direct request through index could be used. With some speed benefits.
