using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyProject.Data;
using MyProject.Pages;

namespace MyProject.Backend
{
    public static class Access
    {
        private static readonly CosmeticsLogeEntities _db = Core.Context;

        public static bool HaveAccess(Users user, RightType requiredPermission)
        {
            return _db.RolesPermissions.Any(rp => rp.RoleId == user.RoleId && rp.Permission == requiredPermission.ToString());
        }

        public static System.Windows.Controls.Page HomePage()
        {
            if (Auth.CurrentUser == null)
            {
                MessageBox.Show("Нет входа в аккаунт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new MainPage();
            }
            switch (Auth.CurrentUser.Roles.Name)
            {
                case "Client": return new CabinetPage();
                case "Master": return new CabinetPage();
                case "Manager": return new CabinetPage();
                case "Admin": return new CabinetPage();
                default: 
                    MessageBox.Show("Неизвестная роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new MainPage();
            }
        }

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
}
