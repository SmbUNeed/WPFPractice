using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Master : IRole
    {
        public string Name { get; set; } = "master";
        public RoleType RoleType { get; set; } = RoleType.Master;
        public List<RightType> Rights { get; set; } = new List<RightType>
        {
            RightType.SignOnMe,
            RightType.CompleteMarkSign,
            RightType.ChooseServiceType,
        };
    }
}
