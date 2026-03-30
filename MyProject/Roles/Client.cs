using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Client : IRole
    {
        
        public string Name { get; set; } = "client";
        public RoleType RoleType { get; set; } = RoleType.Client;
        public List<RightType> Rights { get; set; } = new List<RightType>
        {   
            RightType.Sign,
            RightType.OrderCosmetic,
            RightType.ViewMyOrdersAndSign,
            RightType.ViewMySign,
        };
        
    }
}
