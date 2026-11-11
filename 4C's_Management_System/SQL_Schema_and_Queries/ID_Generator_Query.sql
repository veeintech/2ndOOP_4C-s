/*
    4C's Management System
    ID Generator Database Objects

    RUN ORDER:
    1. Run 4C's_Tables_Schema.sql first.
    2. Run this file after BranchTbl has been created.
    3. Branch records do not need to exist yet. The counter for each
       prefix/branch combination is created automatically the first
       time an ID is requested.

    GENERAL ID FORMAT:
        PREFIX-BRANCHNUMBER-SEQUENCENUMBER

    Example:
        CUS-000001-000001
        TEN-000001-000001

    BRANCH ID FORMAT:
        BRN-000001
*/


/* =========================================================
   1. COUNTER TABLE FOR GENERAL IDs
   One counter per Prefix + BranchId combination.
   ========================================================= */

CREATE TABLE [dbo].[IDSequenceTbl] (
    [Prefix]     VARCHAR (10) NOT NULL,
    [BranchId]   VARCHAR (15) NOT NULL,
    [LastNumber] BIGINT       CONSTRAINT [DF_IDSequenceTbl_LastNumber] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IDSequenceTbl] PRIMARY KEY CLUSTERED ([Prefix] ASC, [BranchId] ASC),
    CONSTRAINT [FK_IDSequenceTbl_BranchTbl] FOREIGN KEY ([BranchId]) REFERENCES [dbo].[BranchTbl] ([BranchId])
);




/* =========================================================
   2. SEQUENCE FOR BRANCH IDs
   This sequence is independent of branch-specific counters.
   ========================================================= */

IF NOT EXISTS
(
    SELECT 1
    FROM sys.sequences
    WHERE name = 'BranchNumberSequence'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE SEQUENCE dbo.BranchNumberSequence
        AS BIGINT
        START WITH 1
        INCREMENT BY 1;
END;
GO


/* =========================================================
   3. PROCEDURE FOR GENERAL IDs
   Creates a counter automatically when a Prefix + BranchId
   combination is used for the first time.

   UPDLOCK + HOLDLOCK helps prevent duplicate numbers when
   multiple users request IDs at the same time.
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.GenerateNextIDNumber
    @Prefix   VARCHAR(10),
    @BranchId VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @NextNumber BIGINT;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.IDSequenceTbl WITH (UPDLOCK, HOLDLOCK)
        SET LastNumber = LastNumber + 1
        WHERE Prefix = @Prefix
          AND BranchId = @BranchId;

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO dbo.IDSequenceTbl
                (Prefix, BranchId, LastNumber)
            VALUES
                (@Prefix, @BranchId, 1);

            SET @NextNumber = 1;
        END
        ELSE
        BEGIN
            SELECT @NextNumber = LastNumber
            FROM dbo.IDSequenceTbl
            WHERE Prefix = @Prefix
              AND BranchId = @BranchId;
        END;

        COMMIT TRANSACTION;

        SELECT @NextNumber AS NextNumber;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


/* =========================================================