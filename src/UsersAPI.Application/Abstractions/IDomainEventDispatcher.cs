using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersAPI.Application.Abstractions
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<object> domainEvents);
    }
}
