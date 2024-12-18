CREATE TABLE Notifications (
    ID int PRIMARY KEY IDENTITY(1,1),
    TypeOfNotification nvarchar(50) NOT NULL,
    CreatedDatetime datetime NOT NULL,
    CreatedBy int NOT NULL,
    CreatedFor int NOT NULL,
    NotificationText nvarchar(500) NOT NULL,
    IsRead bit NOT NULL DEFAULT 0,
    IsDeleted bit NOT NULL DEFAULT 0,
    DataID int null,
    CreatedByName nvarchar(50) null,
    CreatedForName nvarchar(50) null,
)

go

CREATE PROCEDURE InsertNotification
    @TypeOfNotification nvarchar(50),
    @CreatedDatetime datetime,
    @CreatedBy int,
    @CreatedFor int,
    @NotificationText nvarchar(500),
    @RelatedDataID int = null
AS
BEGIN
    INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, RelatedDataID)
    VALUES (@TypeOfNotification, @CreatedDatetime, @CreatedBy, @CreatedFor, @NotificationText, @RelatedDataID)
END
GO

CREATE PROCEDURE UpdateNotification
    @ID int,
    @TypeOfNotification nvarchar(50),
    @CreatedDatetime datetime,
    @CreatedBy int,
    @CreatedFor int,
    @NotificationText nvarchar(500),
    @IsRead bit,
    @IsDeleted bit,
    @RelatedDataID int = null
AS
BEGIN
    UPDATE Notifications
    SET TypeOfNotification = @TypeOfNotification,
        CreatedDatetime = @CreatedDatetime,
        CreatedBy = @CreatedBy,
        CreatedFor = @CreatedFor,
        NotificationText = @NotificationText,
        IsRead = @IsRead,
        IsDeleted = @IsDeleted,
        RelatedDataID = @RelatedDataID
    WHERE ID = @ID
END
GO

CREATE PROCEDURE DeleteNotification
    @ID int
AS
BEGIN
    DELETE FROM Notifications
    WHERE ID = @ID
END
GO

CREATE PROCEDURE SelectNotifications
    @ID int = null,
    @TypeOfNotification nvarchar(50) = null,
    @CreatedBy int = null,
    @CreatedFor int = null
AS
BEGIN
    SELECT *
    FROM Notifications
    WHERE (@ID IS NULL OR ID = @ID)
        AND (@TypeOfNotification IS NULL OR TypeOfNotification = @TypeOfNotification)
        AND (@CreatedBy IS NULL OR CreatedBy = @CreatedBy)
        AND (@CreatedFor IS NULL OR CreatedFor = @CreatedFor)
END
GO

CREATE PROCEDURE GetNotificationByUserID
    @UserID int,
    @StartDate datetime = NULL,
    @EndDate datetime = NULL,
    @Status bit = NULL
AS
BEGIN
    SELECT *
    FROM Notifications
    WHERE CreatedFor = @UserID
        AND (@StartDate IS NULL OR CreatedDatetime >= @StartDate)
        AND (@EndDate IS NULL OR CreatedDatetime <= @EndDate)
        AND (@Status IS NULL OR IsRead = @Status)
END
