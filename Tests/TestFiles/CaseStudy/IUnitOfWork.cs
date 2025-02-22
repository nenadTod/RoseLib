using RentApp.Models.Entities;
using RentApp.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IServiceRepository Services { get; set; }
        IBranchRepository Branches { get; set; }
        ICommentRepository Comments { get; set; }
        IAppUserRepository AppUsers { get; set; }
        IRAIdentityUserRepository Users { get; set; }
        IRolesRepository Roles { get; set; }
        INotificationRepository Notifications { get; set; }
        IItemRepository Items { get; set; }
        INotificationTypeRepository NotificationTypes { get; set; }
        IPricelistRepository Pricelists { get; set; }
        IReservationRepository Reservations { get; set; }
        IVehicleRepository Vehicles { get; set; }
        IVehicleImageRepository VehicleImages { get; set; }
        int Complete();
    }
}
