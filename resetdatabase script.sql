CREATE PROCEDURE sp_DeleteAllTablesAndSPs
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TableName VARCHAR(50)
    DECLARE @SPName VARCHAR(50)

        SELECT name
        FROM sys.tables
        WHERE name IN ('users', 'topics', 'comments', 'messages', 'followings', 'votes', 'watchlist', 'notifications', 'sessions')

    DECLARE curSPs CURSOR LOCAL FOR
        SELECT name
        FROM sys.procedures
        WHERE name LIKE 'sp_%'

    OPEN curTables

    FETCH NEXT FROM curTables INTO @TableName

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC ('DROP TABLE ' + @TableName)

        FETCH NEXT FROM curTables INTO @TableName
    END

    CLOSE curTables
    DEALLOCATE curTables

    OPEN curSPs

    FETCH NEXT FROM curSPs INTO @SPName

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC ('DROP PROCEDURE ' + @SPName)

        FETCH NEXT FROM curSPs INTO @SPName
    END

    CLOSE curSPs
    DEALLOCATE curSPs

END
go


/* call this to delete all tables and SP */
EXEC sp_DeleteAllTablesAndSPs
go
