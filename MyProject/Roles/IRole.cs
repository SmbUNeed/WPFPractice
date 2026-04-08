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
        string Name { get; }
        RoleType RoleType { get; }
        List<RightType> Rights { get; }
    }
}
