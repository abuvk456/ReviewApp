CREATE TABLE Messages (
    MessageID INT PRIMARY KEY IDENTITY(1,1),
    SentBy INT NOT NULL,
    SentTo INT NOT NULL,
    SentDatetime DATETIME NOT NULL,
    ReadDatetime DATETIME,
    IsDeleted BIT NOT NULL DEFAULT 0,
    IsFlaged BIT NOT NULL DEFAULT 0,
    MessageText NVARCHAR(MAX)
)
GO
create PROCEDURE sp_InsertMessage
    @SentBy INT,
    @SentTo INT,
    @SentDatetime DATETIME,
    @MessageText NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SenderName NVARCHAR(100);
    SET @SenderName = (SELECT FirstName FROM Users WHERE userid = @SentBy);

    DECLARE @Message NVARCHAR(MAX);
    SET @Message = @MessageText;

    -- Insert message into Messages table and retrieve the generated MessageId
    DECLARE @MessageId INT;
    INSERT INTO Messages (SentBy, SentTo, SentDatetime, MessageText)
    VALUES (@SentBy, @SentTo, @SentDatetime, @Message);
    SET @MessageId = SCOPE_IDENTITY(); -- Get the generated MessageId

    -- Insert notification with the related MessageId
    INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, RelatedDataID)
    VALUES ('New Message', @SentDatetime, @SentBy, @SentTo, @Message, @MessageId);
END
GO



-- Update Message
CREATE PROCEDURE sp_UpdateMessage
    @MessageID INT,
    @SentBy INT,
    @SentTo INT,
    @SentDatetime DATETIME,
    @ReadDatetime DATETIME,
    @IsDeleted BIT,
    @IsFlaged BIT,
    @MessageText NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Messages
    SET SentBy = @SentBy,
        SentTo = @SentTo,
        SentDatetime = @SentDatetime,
        ReadDatetime = @ReadDatetime,
        IsDeleted = @IsDeleted,
        IsFlaged = @IsFlaged,
        MessageText = @MessageText
    WHERE MessageID = @MessageID;
END
GO

-- Delete Message
CREATE PROCEDURE sp_DeleteMessage
    @MessageID INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Messages WHERE MessageID = @MessageID;
END
GO
create PROCEDURE sp_GetMessagesByUser
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.Email as SentByEmail, R.email as SentToEmail, m.MessageID, m.SentBy, s.FirstName + ' ' + s.LastName AS SentByName, m.SentTo, r.FirstName + ' ' + r.LastName AS SentToName, m.SentDatetime, m.ReadDatetime, m.IsDeleted, m.IsFlaged, m.MessageText
    FROM Messages m
    JOIN Users s ON m.SentBy = s.UserID
    JOIN Users r ON m.SentTo = r.UserID
    WHERE m.MessageID IN (
        SELECT MAX(MessageID)
        FROM Messages
        WHERE (SentBy = @UserID OR SentTo = @UserID)
        GROUP BY CASE WHEN SentBy = @UserID THEN SentTo ELSE SentBy END
    )
    ORDER BY m.SentDatetime DESC;
END
go

CREATE PROCEDURE sp_GetConversationByUsers
    @User1 INT,
    @User2 INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        m.MessageID, 
        m.SentBy, 
        m.SentTo, 
        m.SentDatetime, 
        m.ReadDatetime, 
        m.IsDeleted, 
        m.IsFlaged, 
        m.MessageText, 
        u1.FirstName + ' ' + u1.LastName  AS SentByName, 
        u2.FirstName + ' ' + u2.LastName AS SentToName
    FROM Messages m
    JOIN Users u1 ON m.SentBy = u1.UserID
    JOIN Users u2 ON m.SentTo = u2.UserID
    WHERE (SentBy = @User1 AND SentTo = @User2) OR (SentBy = @User2 AND SentTo = @User1)
    ORDER BY SentDatetime;
END
GO

