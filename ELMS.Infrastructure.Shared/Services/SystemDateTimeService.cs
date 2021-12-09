using ELMCOM.Application.Interfaces.Shared;
using System;

namespace ELMCOM.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}