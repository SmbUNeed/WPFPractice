using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public enum RightType
    {
        Sign,
        OrderCosmetic,
        ViewMyOrdersAndSign,
        ViewMySign,

        SignOnMe,
        CompleteMarkSign,
        ChooseServiceType,

        CreateSign,
        ViewAndGiveOrders,
        CarrySign,
        CancelSign,
        AddCosmeticSales,
        AddCosmeticStock,
        ChangeCosmeticStock,
        DeleteCosmeticStock,
        AddProductType,
        ChangeProductType,
        AddManufacturer,
        ChangeManufacturer,
        AddServiceType,
        ChangeServiceType,

        ChangeUsers,
        AddUsers,
        DeleteUsers,
        ChangeRoles
    }
}
