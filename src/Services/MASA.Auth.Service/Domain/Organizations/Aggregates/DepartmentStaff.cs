﻿namespace MASA.Auth.Service.Domain.Organizations.Aggregates
{
    public class DepartmentStaff : Entity<Guid>
    {
        public Guid DepartmentId { get; private set; }

        public Guid StaffId { get; private set; }

        public Guid UserId { get; private set; }

        public DepartmentStaff(Guid departmentId, Guid staffId, Guid userId)
        {
            DepartmentId = departmentId;
            StaffId = staffId;
            UserId = userId;
        }
    }
}