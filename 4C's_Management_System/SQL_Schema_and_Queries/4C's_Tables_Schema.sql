-- ==========================================
-- 1. BranchTbl
-- ==========================================

CREATE TABLE [dbo].[BranchTbl] (
    [BranchId]   VARCHAR (15)  NOT NULL,
    [BranchName] VARCHAR (100) NOT NULL,
    [Address]    VARCHAR (255) NOT NULL,
    [Status]     VARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_BranchTbl] PRIMARY KEY CLUSTERED ([BranchId] ASC)
);

-- ==========================================
-- 2. TBLRoom
-- ==========================================

CREATE TABLE [dbo].[TBLRoom] (
    [RoomId]   VARCHAR (20)    NOT NULL,
    [BranchId] VARCHAR (15)    NOT NULL,
    [RoomType] VARCHAR (20)    NOT NULL,
    [Capacity] VARCHAR (20)    NOT NULL,
    [Rate]     DECIMAL (10, 2) NOT NULL,
    [Status]   VARCHAR (15)    NOT NULL,
    CONSTRAINT [PK_TBLRoom] PRIMARY KEY CLUSTERED ([RoomId] ASC),
    CONSTRAINT [FK_TBLRoom_BranchTbl] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[BranchTbl] ([BranchId]),
    CONSTRAINT [CK_TBLRoom_Rate] CHECK ([Rate] >= 0)
);

-- ==========================================
-- 3. CustomerTbl
-- ==========================================

CREATE TABLE [dbo].[CustomerTbl] (
    [CustomerID]     VARCHAR (20)  NOT NULL,
    [BranchId]       VARCHAR (15)  NOT NULL,
    [Fullname]       VARCHAR (100) NOT NULL,
    [ContactNumber]  VARCHAR (20)  NOT NULL,
    [Address]        VARCHAR (200) NOT NULL,
    [CustomerType]   VARCHAR (50)  NOT NULL,
    [CustomerStatus] VARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_CustomerTbl] PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    CONSTRAINT [FK_CustomerTbl_BranchTbl] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[BranchTbl] ([BranchId])
);

-- ==========================================
-- 4. TenantTbl
-- ==========================================

CREATE TABLE [dbo].[TenantTbl] (
    [TenantId]           VARCHAR (20)    NOT NULL,
    [BranchId]           VARCHAR (15)    NOT NULL,
    [FullName]           VARCHAR (100)   NOT NULL,
    [ContactNumber]      VARCHAR (20)    NOT NULL,
    [Address]            VARCHAR (200)   NOT NULL,
    [AgreementStartDate] DATE            NOT NULL,
    [AgreementEndDate]   DATE            NOT NULL,
    [MonthlyRent]        DECIMAL (10, 2) NOT NULL,
    [Status]             VARCHAR (20)    NOT NULL,
    CONSTRAINT [PK_TenantTbl] PRIMARY KEY CLUSTERED ([TenantId] ASC),
    CONSTRAINT [FK_TenantTbl_BranchTbl] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[BranchTbl] ([BranchId]),
    CONSTRAINT [CK_TenantTbl_AgreementDates] CHECK ([AgreementEndDate] > [AgreementStartDate]),
    CONSTRAINT [CK_TenantTbl_MonthlyRent] CHECK ([MonthlyRent] >= 0)
);

-- ==========================================
-- 5. UserTbl
-- ==========================================

CREATE TABLE [dbo].[UserTbl] (
    [UserId]   VARCHAR (20)  NOT NULL,
    [BranchId] VARCHAR (15)  NOT NULL,
    [UserName] VARCHAR (30)  NOT NULL,
    [Password] VARCHAR (255) NOT NULL,
    [Role]     VARCHAR (20)  NOT NULL,
    [Status]   VARCHAR (10)  NOT NULL,
    CONSTRAINT [PK_UserTbl] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_UserTbl_BranchTbl] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[BranchTbl] ([BranchId]),
    CONSTRAINT [UQ_UserTbl_UserName] UNIQUE ([UserName])
);

-- ==========================================
-- 6. ReservationTbl
-- ==========================================

CREATE TABLE [dbo].[ReservationTbl] (
    [ReservationId]     VARCHAR (20)  NOT NULL,
    [branchId]          VARCHAR (15)  NOT NULL,
    [CustomerId]        VARCHAR (20)  NULL,
    [TenantId]          VARCHAR (20)  NULL,
    [RoomId]            VARCHAR (20)  NOT NULL,
    [AccomodationType]  VARCHAR (30)  NOT NULL,
    [ReservationDate]   DATETIME2 (7) NOT NULL,
    [ResStartDate]      DATETIME2 (7) NOT NULL,
    [ResEndDate]        DATETIME2 (7) NOT NULL,
    [ReservationStatus] VARCHAR (20)  NULL,
    CONSTRAINT [PK_ReservationTbl] PRIMARY KEY CLUSTERED ([ReservationId] ASC),
    CONSTRAINT [FK_ReservationTbl_TBLRoom] FOREIGN KEY ([RoomId]) REFERENCES [dbo].[TBLRoom] ([RoomId]),
    CONSTRAINT [FK_ReservationTbl_TenantTbl] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[TenantTbl] ([TenantId]),
    CONSTRAINT [FK_ReservationTbl_BranchTbl] FOREIGN KEY ([branchId]) REFERENCES [dbo].[BranchTbl] ([BranchId]),
    CONSTRAINT [FK_ReservationTbl_CustomerTbl] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerTbl] ([CustomerID]),
    CONSTRAINT [CK_ReservationTbl_Dates] CHECK ([ResEndDate] > [ResStartDate]),
    CONSTRAINT [CK_ReservationTbl_Owner] CHECK (
        ([CustomerId] IS NOT NULL AND [TenantId] IS NULL)
        OR ([CustomerId] IS NULL AND [TenantId] IS NOT NULL)
    )
);

-- ==========================================
-- 7. MaintenanceTbl
-- ==========================================

CREATE TABLE [dbo].[MaintenanceTbl]
(
    [MaintenanceID] VARCHAR(20) PRIMARY KEY,
	[BranchID] VARCHAR(15), 
    [RoomID] VARCHAR(20) NOT NULL,
    [Issue] NVARCHAR(150) NOT NULL,
    [DateStarted] DATETIME NULL,
    [DateCompleted]	 DATETIME NULL,
    [Remarks] NVARCHAR(500),
    [Status] NVARCHAR(30) NOT NULL DEFAULT 'Pending',
    FOREIGN KEY ([BranchID]) REFERENCES [dbo].[BranchTbl]([BranchID])
);

-- ==========================================
-- 8. ReservationTBl
-- ==========================================

CREATE TABLE [dbo].[PaymentTbl] (
    [PaymentID]       VARCHAR(20) NOT NULL,
    [BranchID]        VARCHAR(15) NOT NULL,
    [ReservationID]   VARCHAR(20) NULL,
    [TenantID]        VARCHAR(20) NULL,
    [CustomerID]      VARCHAR(20) NULL,
    [AmountPaid]      DECIMAL(10,2) NULL,
    [PaymentDate]     DATETIME2(7) DEFAULT (GETDATE()) NULL,
    [PaymentMethod]   VARCHAR(30) NULL,
    [ReferenceNumber] VARCHAR(20) NULL,
    [Balance]         DECIMAL(10,2) NULL,
    [PaymentStatus]   VARCHAR(20) DEFAULT ('Pending') NOT NULL,

    CONSTRAINT PK_PaymentTbl
        PRIMARY KEY CLUSTERED ([PaymentID] ASC),

    CONSTRAINT FK_PaymentTbl_BranchTbl
        FOREIGN KEY ([BranchID])
        REFERENCES [dbo].[BranchTbl] ([BranchID]),

    CONSTRAINT FK_PaymentTbl_ReservationTbl
        FOREIGN KEY ([ReservationID])
        REFERENCES [dbo].[ReservationTbl] ([ReservationID]),

    CONSTRAINT FK_PaymentTbl_TenantTbl
        FOREIGN KEY ([TenantID])
        REFERENCES [dbo].[TenantTbl] ([TenantID]),

    CONSTRAINT FK_PaymentTbl_CustomerTbl
        FOREIGN KEY ([CustomerID])
        REFERENCES [dbo].[CustomerTbl] ([CustomerID])
);

-- ==========================================
-- 9. PaymenytTBl
-- ==========================================

CREATE TABLE [dbo].[StaffTbl]
(
    [StaffID] VARCHAR(20) NOT NULL PRIMARY KEY,
    [BranchID] VARCHAR(15) NOT NULL,
    [FullName] VARCHAR(100) NOT NULL,
    [ContactNumber] VARCHAR(12) NOT NULL,
    [Email] VARCHAR(40) NOT NULL,
    [Adress] VARCHAR(100) NOT NULL,
    [Gender] CHAR(1) NOT NULL,
    [JobPosition] VARCHAR(30),
    [Datehired] DATETIME,
    [SalaryRate] DECIMAL(10,2),
    [Status] VARCHAR(30) NOT NULL,

    CONSTRAINT FK_StaffTbl_BranchTbl
    FOREIGN KEY (BranchID)
    REFERENCES BranchTbl(BranchID)
);