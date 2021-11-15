using ELMS.Application.Interfaces.Shared;
using System;

namespace ELMS.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}