using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Client : IRole
    {
        
        public string Name { get; } = "client";
        public RoleType RoleType { get; } = RoleType.Client;
        public List<RightType> Rights { get; } = new List<RightType>
        {   
            RightType.Appoint,
            RightType.OrderCosmetic,
            RightType.ViewMyOrders,
            RightType.ViewMyAppointments,
        };
    }
}
