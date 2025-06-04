using Shared.Interfaces;
using Shared.Models;
using Shared.Repository;

namespace Shared.Services
{

	public class SessionManagement
	{
		private readonly IGenericRepository<Session> _sessionRepository;
		public SessionManagement(IGenericRepository<Session> sessionRepository)
		{
			_sessionRepository = sessionRepository;
		}

		public async Task SessionStart(string userId)
		{
			await _sessionRepository.Add(new Session { UserId = userId, StartAt = DateTime.Now});
		}

		public async Task SessionStop(string userId)
		{
			var session = await (_sessionRepository as SessionRepository).GetLastSession(userId);
			if (session != null)
			{
				session.EndTime = DateTime.Now;
				await _sessionRepository.Update(session);
			}
		}

		public Task<Session> GetSession(string userId)
		{
			return _sessionRepository.GetById(userId);
		}	
	}
}