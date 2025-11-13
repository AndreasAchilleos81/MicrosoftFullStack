using System.Text.RegularExpressions;

namespace SafeVault.Helpers
{
	public static class ValidationHelpers
	{
		public static bool IsValidInput(string input, string allowedSpecialCharacters = "")
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}

			var validCharacters = new HashSet<char>(allowedSpecialCharacters);
			return input.All(c => char.IsLetterOrDigit(c) || validCharacters.Contains(c));
		}

		public static bool IsValidXssInput(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return true;
			}

			string lowered = input.ToLower();
			return !(lowered.Contains("<script") || lowered.Contains("<iframe"));
		}

		public static string SanitizeInput(string input, string allowedSpecialCharacters = "")
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			// Remove script and iframe tags
			string sanitized = Regex.Replace(input, @"<script.*?>.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			sanitized = Regex.Replace(sanitized, @"<iframe.*?>.*?</iframe>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Remove all characters not alphanumeric or in allowed set
			var validCharacters = new HashSet<char>(allowedSpecialCharacters);
			sanitized = new string(sanitized.Where(c => char.IsLetterOrDigit(c) || validCharacters.Contains(c)).ToArray());

			return sanitized;
		}
	}
}
