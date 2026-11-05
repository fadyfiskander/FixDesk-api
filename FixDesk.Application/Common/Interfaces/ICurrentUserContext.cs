using FixDesk.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Application.Common.Interfaces
{
    public interface ICurrentUserContext
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
        UserRole Role { get; }
        bool IsOwner { get; }
    }}
