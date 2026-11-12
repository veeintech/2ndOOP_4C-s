
-- ==========================================
-- 4C's MANAGEMENT SYSTEM
-- TEST DATA / CONTENT QUERIES
-- ==========================================

-- 1. BranchTbl
IF NOT EXISTS (
    SELECT 1 FROM dbo.BranchTbl
    WHERE BranchId = 'BRC-002'
)
BEGIN
    INSERT INTO dbo.BranchTbl
        (BranchId, BranchName, Address, Status)
    VALUES
        ('BRC-002', 'Naujan Main Branch',
         'Poblacion, Naujan, Oriental Mindoro', 'Active');
END;


-- 2. TBLRoom
INSERT INTO dbo.TBLRoom
    (RoomId, BranchId, RoomType, Capacity, Rate, Status)
SELECT
    V.RoomId, V.BranchId, V.RoomType,
    V.Capacity, V.Rate, V.Status
FROM (VALUES
    -- HOTEL
    ('RM-002-002', 'BRC-002', 'Hotel', 'Double Room', 1500.00, 'Available'),
    ('RM-002-003', 'BRC-002', 'Hotel', 'Single Room', 1000.00, 'Occupied'),
    ('RM-002-004', 'BRC-002', 'Hotel', 'Double Room', 1500.00, 'Occupied'),
    ('RM-002-005', 'BRC-002', 'Hotel', 'Single Room', 1000.00, 'Reserved'),
    ('RM-002-006', 'BRC-002', 'Hotel', 'Double Room', 1500.00, 'Maintenance'),
    ('RM-002-007', 'BRC-002', 'Hotel', 'Single Room', 1000.00, 'Available'),
    ('RM-002-008', 'BRC-002', 'Hotel', 'Double Room', 1500.00, 'Reserved'),

    -- APARTMENT
    ('RM-002-009', 'BRC-002', 'Apartment', 'Single Room', 1200.00, 'Available'),
    ('RM-002-010', 'BRC-002', 'Apartment', 'Double Room', 1800.00, 'Occupied'),
    ('RM-002-011', 'BRC-002', 'Apartment', 'Single Room', 1200.00, 'Reserved'),
    ('RM-002-012', 'BRC-002', 'Apartment', 'Double Room', 1800.00, 'Maintenance'),
    ('RM-002-013', 'BRC-002', 'Apartment', 'Single Room', 1200.00, 'Available'),
    ('RM-002-014', 'BRC-002', 'Apartment', 'Double Room', 1800.00, 'Occupied'),
    ('RM-002-015', 'BRC-002', 'Apartment', 'Single Room', 1200.00, 'Reserved'),
    ('RM-002-016', 'BRC-002', 'Apartment', 'Double Room', 1800.00, 'Available'),

    -- TRANSIENT
    ('RM-002-017', 'BRC-002', 'Transient', 'Single Room', 500.00, 'Available'),
    ('RM-002-018', 'BRC-002', 'Transient', 'Double Room', 800.00, 'Occupied'),
    ('RM-002-019', 'BRC-002', 'Transient', 'Single Room', 500.00, 'Reserved'),
    ('RM-002-020', 'BRC-002', 'Transient', 'Double Room', 800.00, 'Maintenance'),
    ('RM-002-021', 'BRC-002', 'Transient', 'Single Room', 500.00, 'Available'),
    ('RM-002-022', 'BRC-002', 'Transient', 'Double Room', 800.00, 'Occupied'),
    ('RM-002-023', 'BRC-002', 'Transient', 'Single Room', 500.00, 'Reserved'),
    ('RM-002-024', 'BRC-002', 'Transient', 'Double Room', 800.00, 'Available')
) AS V(RoomId, BranchId, RoomType, Capacity, Rate, Status)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.TBLRoom R
    WHERE R.RoomId = V.RoomId
);


-- 3. UserTbl
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserTbl
    WHERE UserId = 'USR-001'
)
BEGIN
    INSERT INTO dbo.UserTbl
        (UserId, BranchId, UserName, Password, Role, Status)
    VALUES
        ('USR-001', 'BRC-002', 'ja', 'ja123',
         'MasterAdmin', 'Active');
END;


-- ==========================================
-- VERIFY INSERTED DATA
-- ==========================================

SELECT * FROM dbo.BranchTbl
WHERE BranchId = 'BRC-002';

SELECT * FROM dbo.TBLRoom
WHERE BranchId = 'BRC-002';

SELECT * FROM dbo.UserTbl
WHERE BranchId = 'BRC-002';
