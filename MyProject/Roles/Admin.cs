using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Admin : IRole
    {
        public string Name { get; set; } = "admin";
        public RoleType RoleType { get; set; } = RoleType.Admin;
        public List<RightType> Rights { get; set; } = new List<RightType>
        {
            RightType.ChangeUsers,
            RightType.AddUsers,
            RightType.DeleteUsers,
            RightType.ChangeRoles
        };
    }
}
