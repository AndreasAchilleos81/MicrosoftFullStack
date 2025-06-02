using Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Shared.Repository
{
	public class GenericRepository<T> : IGenericRepository<T>, IDisposable
	{
		protected readonly IConfiguration _configuration;
		protected readonly SqliteConnection _connection;
		protected readonly ILogger<T> _logger;
		private bool disposed = false;

		public GenericRepository(IConfiguration configuration, ILogger<T> logger)
		{
			_logger = logger;
			Batteries.Init();
			_configuration = configuration;
			var relativeLocation = _configuration["Logging:ConnectionStrings:DefaultConnection"].TrimEnd(';');
			var absolutePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeLocation));
			var connectionString = $"Data Source={absolutePath};";
			_connection = new SqliteConnection(connectionString);
			_connection.Open();
		}

		public async Task<bool> Add(T entity)
		{
			int rowsAffected = 0;
			try
			{
				string tableName = GetTableName();
				string columns = GetColumns(excludekey: false);
				string properties = GetPropertyNames(excludekey: false);
				string query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties})";
				rowsAffected = await _connection.ExecuteAsync(query, entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}
			return rowsAffected > 0 ? true : false;
		}

		public async Task<bool> Delete(T entity)
		{
			int rowsAffected = 0;
			try
			{
				string tableName = GetTableName();
				string keyColumn = GetKeyColumnName();
				string keyProperty = GetKeyPropertyName();
				string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyProperty}";
				rowsAffected = await _connection.ExecuteAsync(query, entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return rowsAffected > 0 ? true : false;
		}

		public async Task<IEnumerable<T>> GetAll()
		{
			IEnumerable<T> result = null;
			try
			{
				string tableName = GetTableName();
				string query = $"SELECT * FROM {tableName}";
				result = await _connection.QueryAsync<T>(query);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return result;
		}

		public async Task<T> GetById(int Id)
		{
			IEnumerable<T> result = null;
			try
			{
				string tableName = GetTableName();
				string keyColumn = GetKeyColumnName();
				string query = $"SELECT  * From {tableName} WHERE {keyColumn} = '{Id}'";
				result = await _connection.QueryAsync<T>(query);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return result.FirstOrDefault();
		}

		public async Task<T> GetById(string Id)
		{
			IEnumerable<T> result = null;
			try
			{
				string tableName = GetTableName();
				string keyColumn = GetKeyColumnName();
				string query = $"SELECT  * From {tableName} WHERE {keyColumn} = '{Id}'";
				result = await _connection.QueryAsync<T>(query);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}


			return result.FirstOrDefault();
		}

		public async Task<bool> Update(T entity)
		{
			int rowsAffected = 0;
			try
			{
				string tableName = GetTableName();
				string keyColumn = GetKeyColumnName();
				string keyProperty = GetKeyPropertyName();

				StringBuilder query = new StringBuilder();
				query.Append($"UPDATE {tableName} SET ");

				foreach (var property in GetProperties(true))
				{
					var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
					string propertyName = property.Name;
					string columnName = columnAttr.Name;

					query.Append($"{columnName} = @{propertyName},");
				}

				query.Remove(query.Length - 1, 1);
				query.Append($" WHERE {keyColumn} = @{keyProperty}");
				rowsAffected = await _connection.ExecuteAsync(query.ToString(), entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return rowsAffected > 0 ? true : false;
		}

		protected string GetTableName()
		{
			string tableName = "";
			var type = typeof(T);
			var tableAttribute = type.GetCustomAttribute<TableAttribute>();
			if (tableAttribute != null)
			{
				tableName = tableAttribute.Name;
				return tableName;
			}

			return tableName + "s";
		}

		public static string GetKeyColumnName()
		{
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

				if (keyAttributes != null && keyAttributes.Length > 0)
				{
					object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

					if (columnAttributes != null && columnAttributes.Length > 0)
					{
						ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
						return columnAttribute.Name;
					}
					else
					{
						return property.Name;
					}
				}
			}

			return null;
		}

		private string GetColumns(bool excludekey = false)
		{
			var type = typeof(T);
			//var columns = string.Join(", ", type.GetProperties()
			//	.Where(p => !excludekey || !p.IsDefined(typeof(KeyAttribute)))
			//	.Select(p =>
			//	{
			//		var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
			//		return columnAttr != null ? columnAttr.Name : p.Name;
			//	}));

			var properties = type.GetProperties();
			if (excludekey)
			{
				properties = properties.Where(p => !p.IsDefined(typeof(KeyAttribute))).ToArray();
			}

			var columnNames = properties
								.Where(p => p.GetCustomAttribute<ColumnAttribute>() != null)
								.Select(c => c.GetCustomAttribute<ColumnAttribute>())
								.Select(j => j.Name);

			var columns = string.Join(", ", columnNames);

			//.Where(p => !excludekey || !p.IsDefined(typeof(KeyAttribute)))
			//.Select(p =>
			//{
			//	var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
			//	return columnAttr != null ? columnAttr.Name : p.Name;
			//}));


			return columns;
		}

		protected string GetPropertyNames(bool excludekey = false)
		{
			//var properties = typeof(T).GetProperties().Where(p => !excludekey || p.GetCustomAttribute<KeyAttribute>() == null);
			//var values = string.Join(", ", properties.Select(p =>
			//{
			//	return $"@{p.Name}"; ;
			//}));

			//return values;

			var properties = typeof(T).GetProperties().ToList(); //.Where(p => !excludekey || p.GetCustomAttribute<KeyAttribute>() == null);
			if (excludekey)
			{
				properties = properties.Where(p => p.GetCustomAttribute<KeyAttribute>() == null).Select(pi => pi).ToList();
			}
			properties = properties.Where(p => p.GetCustomAttribute<ColumnAttribute>() != null).ToList();

			var values = string.Join(", ", properties.Select(p =>
			{
				return $"@{p.Name}"; ;
			}));

			return values;
		}

		// excludeKey = false returns all properties including the ones that are of keyAttribute
		// excludeKey = true returns all properties that do not have the keyAttribute therefore all propeties that are not the primary key of the table row entry
		protected IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
		{
			var properties = typeof(T).GetProperties()
									 .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);
			return properties;
		}

		protected string GetKeyPropertyName()
		{
			var properties = typeof(T).GetProperties()
									.Where(propa => propa.GetCustomAttributes<KeyAttribute>() != null);

			if (properties.Any())
			{
				return properties.FirstOrDefault()?.Name;
			}

			return null;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_connection.Close();
				}
				disposed = true;
			}
		}
	}
}