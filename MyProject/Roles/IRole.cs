using MyProject.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public interface IRole
    {
        string Name { get; set; }
        RoleType RoleType { get; set; }
        List<RightType> Rights { get; set; }
    }

}
