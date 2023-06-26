namespace ComputationalGeometry;

public static class ListExtensions
{
	public static IEnumerable<(T item1, T item2)> DistinctPairs<T>(this IList<T> list)
	{

		for (int i = 0; i < list.Count; i++)
		{
			for (int j = i + 1; j < list.Count; j++)
			{
				yield return (list[i], list[j]);
			}
		}
	}

	public static IEnumerable<(T item1, T item2)> DistinctPairs<T>(this IEnumerable<T> list) =>
		list is IList<T> indexableList 
			? indexableList.DistinctPairs() 
			: list.ToList().DistinctPairs();

	public static IEnumerable<TResult> Select<T1, T2, TResult>(this IEnumerable<(T1 item1, T2 item2)> source, Func<T1, T2, TResult> mapping)
		=> source.Select(pair => mapping(pair.item1, pair.item2));
}
