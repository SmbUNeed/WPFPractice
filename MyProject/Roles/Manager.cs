using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public class Manager : IRole
    {
        public string Name { get; set; } = "manager";
        public RoleType RoleType { get; set; } = RoleType.Manager;
        public List<RightType> Rights { get; set; } = new List<RightType>
        {
            RightType.CreateSign,
            RightType.ViewAndGiveOrders,
            RightType.CarrySign,
            RightType.CancelSign,
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
