using BinaryUserSearch.Types;

namespace BinaryUserSearch
{
    public class BinarySearch
    {
		public Result Search(Users users, string targetUsername)
		{
			// Validate input
			if (users == null || users.results.Count() == 0)
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(targetUsername))
			{
				return null;
			}

			// Binary search requires sorted data
			users.results = users.results.OrderBy(u => u.name.first).ToArray();


			int left = 0;
			int right = users.results.Count() - 1;

			while (left <= right)
			{
				int mid = left + (right - left) / 2;
				int comparison = string.Compare(
					users.results[mid].name.first,
					targetUsername,
					StringComparison.OrdinalIgnoreCase
				);

				if (comparison == 0)
				{
					return users.results[mid];
				}

				if (comparison < 0)
				{
					left = mid + 1;
				}
				else
				{
					right = mid - 1;
				}
			}

			return null;
		}
	}
}
