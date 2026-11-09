
-- ==========================================
-- 4C's MANAGEMENT SYSTEM
-- TEST DATA / CONTENT QUERIES
-- ==========================================

-- ==========================================
-- 1. BranchTbl
-- ==========================================

INSERT INTO [dbo].[BranchTbl]
    ([BranchId], [BranchName], [Address], [Status])
VALUES
    ('BRN-001', 'Naujan Main Branch', 'Poblacion, Naujan, Oriental Mindoro', 'Active'),
    ('BRN-002', 'Calapan Branch', 'Calapan City, Oriental Mindoro', 'Active'),
    ('BRN-003', 'Victoria Branch', 'Victoria, Oriental Mindoro', 'Inactive');
GO


-- ==========================================
-- 2. TBLRoom
-- ==========================================

INSERT INTO [dbo].[TBLRoom]
    ([RoomId], [BranchId], [RoomType], [Capacity], [Rate], [Status])
VALUES
    ('RM-001', 'BRN-001', 'Standard', '2', 800.00, 'Available'),
    ('RM-002', 'BRN-001', 'Deluxe', '3', 1500.00, 'Available'),
    ('RM-003', 'BRN-001', 'Family', '5', 2500.00, 'Occupied'),
    ('RM-004', 'BRN-001', 'Transient', '2', 600.00, 'Available'),
    ('RM-005', 'BRN-001', 'Apartment', '4', 8000.00, 'Occupied'),

    ('RM-006', 'BRN-002', 'Standard', '2', 900.00, 'Available'),
    ('RM-007', 'BRN-002', 'Deluxe', '3', 1800.00, 'Available'),
    ('RM-008', 'BRN-002', 'Family', '5', 3000.00, 'Maintenance'),

    ('RM-009', 'BRN-003', 'Standard', '2', 750.00, 'Available'),
    ('RM-010', 'BRN-003', 'Apartment', '4', 7500.00, 'Available');
GO


-- ==========================================
-- 3. CustomerTbl
-- ==========================================

INSERT INTO [dbo].[CustomerTbl]
    ([CustomerID], [BranchId], [Fullname], [ContactNumber],
     [Address], [CustomerType], [CustomerStatus])
VALUES
    ('CUS-001', 'BRN-001', 'Juan Dela Cruz', '09171234567',
     'Naujan, Oriental Mindoro', 'Hotel', 'Active'),

    ('CUS-002', 'BRN-001', 'Maria Santos', '09181234567',
     'Calapan City, Oriental Mindoro', 'Transient', 'Active'),

    ('CUS-003', 'BRN-001', 'Pedro Reyes', '09191234567',
     'Victoria, Oriental Mindoro', 'Hotel', 'Active'),

    ('CUS-004', 'BRN-002', 'Ana Garcia', '09201234567',
     'Pinamalayan, Oriental Mindoro', 'Hotel', 'Active'),

    ('CUS-005', 'BRN-002', 'Mark Mendoza', '09211234567',
     'Baco, Oriental Mindoro', 'Transient', 'Inactive');
GO


-- ==========================================
-- 4. TenantTbl
-- ==========================================

INSERT INTO [dbo].[TenantTbl]
    ([TenantId], [BranchId], [FullName], [ContactNumber],
     [Address], [AgreementStartDate], [AgreementEndDate],
     [MonthlyRent], [Status])
VALUES
    ('TEN-001', 'BRN-001', 'Carlos Fernandez', '09171230001',
     'Naujan, Oriental Mindoro',
     '2026-01-01', '2026-12-31', 8000.00, 'Active'),

    ('TEN-002', 'BRN-001', 'Sofia Ramirez', '09181230002',
     'Calapan City, Oriental Mindoro',
     '2026-06-01', '2027-05-31', 10000.00, 'Active'),

    ('TEN-003', 'BRN-002', 'Daniel Cruz', '09191230003',
     'Victoria, Oriental Mindoro',
     '2025-01-01', '2025-12-31', 7500.00, 'Expired');
GO


-- ==========================================
-- 5. UserTbl
-- ==========================================

INSERT INTO [dbo].[UserTbl]
    ([UserId], [BranchId], [UserName], [Password], [Role], [Status])
VALUES
    ('USR-001', 'BRN-001', 'masteradmin', 'TestMaster123',
     'MasterAdmin', 'Active'),

    ('USR-002', 'BRN-001', 'admin1', 'TestAdmin123',
     'Admin', 'Active'),

    ('USR-003', 'BRN-002', 'admin2', 'TestAdmin123',
     'Admin', 'Active'),

    ('USR-004', 'BRN-001', 'staff1', 'TestStaff123',
     'Staff', 'Pending'),

    ('USR-005', 'BRN-002', 'staff2', 'TestStaff123',
     'Staff', 'Active');
GO


-- ==========================================
-- 6. ReservationTbl
-- ==========================================

INSERT INTO [dbo].[ReservationTbl]
    ([ReservationId], [branchId], [CustomerId], [TenantId],
     [RoomId], [AccomodationType], [ReservationDate],
     [ResStartDate], [ResEndDate], [ReservationStatus])
VALUES

    -- Reserved
    ('RES-001', 'BRN-001', 'CUS-001', NULL,
     'RM-001', 'Hotel', GETDATE(),
     DATEADD(DAY, 2, GETDATE()),
     DATEADD(DAY, 4, GETDATE()), 'Reserved'),

    -- Checked In
    ('RES-002', 'BRN-001', 'CUS-002', NULL,
     'RM-003', 'Hotel', GETDATE(),
     DATEADD(DAY, -1, GETDATE()),
     DATEADD(DAY, 2, GETDATE()), 'Checkin'),

    -- Checked Out
    ('RES-003', 'BRN-001', 'CUS-003', NULL,
     'RM-004', 'Transient', GETDATE(),
     DATEADD(DAY, -5, GETDATE()),
     DATEADD(DAY, -3, GETDATE()), 'Checkout'),

    -- Cancelled
    ('RES-004', 'BRN-002', 'CUS-004', NULL,
     'RM-006', 'Hotel', GETDATE(),
     DATEADD(DAY, 5, GETDATE()),
     DATEADD(DAY, 7, GETDATE()), 'Cancel'),

    -- Another Reservation
    ('RES-005', 'BRN-002', 'CUS-005', NULL,
     'RM-007', 'Hotel', GETDATE(),
     DATEADD(DAY, 3, GETDATE()),
     DATEADD(DAY, 5, GETDATE()), 'Reserved'),

    -- Tenant Reservation
    ('RES-006', 'BRN-001', NULL, 'TEN-001',
     'RM-005', 'Apartment', GETDATE(),
     DATEADD(DAY, -30, GETDATE()),
     DATEADD(DAY, 335, GETDATE()), 'Checkin'),

    -- Another Tenant Reservation
    ('RES-007', 'BRN-001', NULL, 'TEN-002',
     'RM-002', 'Apartment', GETDATE(),
     DATEADD(DAY, -10, GETDATE()),
     DATEADD(DAY, 355, GETDATE()), 'Checkin'),

    -- Another Branch Reservation
    ('RES-008', 'BRN-002', NULL, 'TEN-003',
     'RM-010', 'Apartment', GETDATE(),
     DATEADD(DAY, -365, GETDATE()),
     DATEADD(DAY, -1, GETDATE()), 'Checkout');
GO


-- ==========================================
-- END OF TEST DATA
-- ==========================================
