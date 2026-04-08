using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Master : IRole
    {
        public string Name { get; } = "master";
        public RoleType RoleType { get; } = RoleType.Master;
        public List<RightType> Rights { get; } = new List<RightType>
        {
            RightType.AppointmentsOnMe,
            RightType.CompleteAppointment,
            RightType.ChooseServiceType,
        };
    }
}
