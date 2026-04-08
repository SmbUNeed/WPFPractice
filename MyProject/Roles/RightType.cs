using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Roles
{
    public enum RightType
    {
        Appoint,
        OrderCosmetic,
        ViewMyOrders,
        ViewMyAppointments,

        AppointmentsOnMe,
        CompleteAppointment,
        ChooseServiceType,

        CreateAppointment,
        ViewAndGiveOrders,
        CarryAppointment,
        CancelAppointment,
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
