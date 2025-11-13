namespace SafeVault.Tests
{
	using NUnit.Framework;
	using SafeVault.Services;
	using static SafeVault.Helpers.ValidationHelpers;

	[TestFixture]
	public class InputValidationTests
	{
		private const string ConnectionString = "Data Source=C:\\Users\\a.achilleos\\code\\MicrosoftFullStack\\8. Security And Authentication\\tryOuts\\SafeVault\\SafeVaultDb";

		private AuthService _authService;

		[SetUp]
		public void Setup()
		{
			_authService = new AuthService(ConnectionString);
		}

		[Test]
		public void Should_Block_SQL_Injection_Attempt()
		{
			// Arrange
			string maliciousUsername = "admin' OR '1'='1";
			string dummyPassword = "password";

			// Act
			bool loginResult = _authService.LoginUser(maliciousUsername, dummyPassword);

			// Assert
			Assert.That(loginResult, Is.False, "❌ SQL Injection attempt was not blocked.");
		}

		[TestCase("<script>alert('XSS')</script>", TestName = "Should_Block_Script_Tag_XSS")]
		[TestCase("<iframe src='evil.com'></iframe>", TestName = "Should_Block_Iframe_XSS")]
		[TestCase("<ScRiPt>alert('XSS')</ScRiPt>", TestName = "Should_Block_CaseInsensitive_Script_Tag")]
		public void Should_Block_XSS_Attempts(string maliciousInput)
		{
			// Act
			bool isValid = IsValidXssInput(maliciousInput);

			// Assert
			Assert.That(isValid, Is.False, $"❌ XSS input '{maliciousInput}' was not blocked.");
		}
	}
}
