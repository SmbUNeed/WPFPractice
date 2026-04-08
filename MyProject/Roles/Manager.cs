using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Manager : IRole
    {
        public string Name { get; } = "manager";
        public RoleType RoleType { get; } = RoleType.Manager;
        public List<RightType> Rights { get; } = new List<RightType>
        {
            RightType.CreateAppointment,
            RightType.ViewAndGiveOrders,
            RightType.CarryAppointment,
            RightType.CancelAppointment,
            RightType.AddCosmeticSales,
            RightType.AddCosmeticStock,
            RightType.ChangeCosmeticStock,
            RightType.DeleteCosmeticStock,
            RightType.AddProductType,
            RightType.ChangeProductType,
            RightType.AddManufacturer,
            RightType.ChangeManufacturer,
            RightType.AddServiceType,
            RightType.ChangeServiceType
        };
    }
}
