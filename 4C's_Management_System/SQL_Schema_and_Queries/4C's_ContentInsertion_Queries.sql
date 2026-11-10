
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
    ('BRN-001', 'Naujan Main Branch', 'Poblacion, Naujan, Oriental Mindoro', 'Active')
GO


-- ==========================================
-- 2. TBLRoom
-- ==========================================

INSERT INTO [dbo].[TBLRoom]
    ([RoomId], [BranchId], [RoomType], [Capacity], [Rate], [Status])
VALUES
    ('RM-001', 'BRN-001', 'Hotel', 'Double Room', 800.00, 'Available'),
    ('RM-002', 'BRN-001', 'Apartment', 'Single Room', 1500.00, 'Available')
GO


-- ==========================================
-- 3. CustomerTbl
-- ==========================================

INSERT INTO [dbo].[CustomerTbl]
    ([CustomerID], [BranchId], [Fullname], [ContactNumber],
     [Address], [CustomerType], [CustomerStatus])
VALUES
    ('CUS-001', 'BRN-001', 'Juan Dela Cruz', '09171234567',
     'Naujan, Oriental Mindoro', 'Hotel', 'Inactive')
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
     '2026-01-01', '2026-12-31', 8000.00, 'Inactive')
GO


-- ==========================================
-- 5. UserTbl
-- ==========================================

INSERT INTO [dbo].[UserTbl]
    ([UserId], [BranchId], [UserName], [Password], [Role], [Status])
VALUES
    ('USR-001', 'BRN-001', 'masteradmin', 'TestMaster123',
     'MasterAdmin', 'Active')
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
     DATEADD(DAY, 4, GETDATE()), 'Reserved')
GO


-- ==========================================
-- END OF TEST DATA
-- ==========================================
