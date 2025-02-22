using RentApp.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Unity.Attributes;

namespace RentApp.Persistance.UnitOfWork
{
    public class RADBUnitOfWork:IUnitOfWork
    {
        private readonly DbContext _context;

        [Dependency]
        public IServiceRepository Services { get; set; }
        [Dependency]
        public IBranchRepository Branches { get; set; }
        [Dependency]
        public ICommentRepository Comments { get; set; }
        [Dependency]
        public IAppUserRepository AppUsers { get; set; }
        [Dependency]
        public IRAIdentityUserRepository Users { get; set; }
        [Dependency]
        public IRolesRepository Roles { get; set; }
        [Dependency]
        public INotificationRepository Notifications { get; set; }
        [Dependency]
        public IItemRepository Items { get; set; }
        [Dependency]
        public INotificationTypeRepository NotificationTypes { get; set; }
        [Dependency]
        public IPricelistRepository Pricelists { get; set; }
        [Dependency]
        public IReservationRepository Reservations { get; set; }
        [Dependency]
        public IVehicleRepository Vehicles { get; set; }
        [Dependency]
        public IVehicleImageRepository VehicleImages { get; set; }

        public RADBUnitOfWork(DbContext context)
        {
            _context = context;
            _context.Configuration.ProxyCreationEnabled = true;
            _context.Configuration.LazyLoadingEnabled = true;

        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}