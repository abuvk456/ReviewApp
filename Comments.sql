CREATE TABLE Comments (
    CommentId INT PRIMARY KEY IDENTITY,
    TopicId INT NOT NULL,
    CommentText NVARCHAR(MAX) NOT NULL,
    CommentedBy INT NOT NULL,
    CommentDateTime DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Comments_IsDeleted DEFAULT 0,
    CONSTRAINT FK_Comments_TopicId FOREIGN KEY (TopicId) REFERENCES Topics (TopicId),
    CONSTRAINT FK_Comments_CommentedBy FOREIGN KEY (CommentedBy) REFERENCES Users (UserId)
);
go
CREATE PROCEDURE sp_CreateComment
    @TopicId INT,
    @CommentText NVARCHAR(MAX),
    @CommentedBy INT,
    @CommentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Comments (TopicId, CommentText, CommentedBy, CommentDateTime, IsDeleted)
    VALUES (@TopicId, @CommentText, @CommentedBy, GETUTCDATE(), 0);

    SET @CommentId = SCOPE_IDENTITY();

    DECLARE @NotificationText NVARCHAR(500);
    SET @NotificationText = CONCAT('New comment added: ', @CommentText);

    INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, RelatedDataId)
    VALUES ('New Comment', GETUTCDATE(), @CommentedBy, @TopicId, @NotificationText, @CommentId);
END;
GO


CREATE PROCEDURE sp_DeleteComment
    @CommentId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM 
        Comments 
    WHERE 
        CommentId = @CommentId;
END;
go


CREATE PROCEDURE sp_UpdateComment
    @CommentId INT,
    @CommentText NVARCHAR(MAX),
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE 
        Comments 
    SET 
        CommentText = @CommentText,
        IsDeleted = @IsDeleted
    WHERE 
        CommentId = @CommentId;
END;

go

CREATE PROCEDURE sp_ReadComment
    @CommentId INT = NULL,
    @CommentedBy INT = NULL,
    @TopicId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CommentId,
        TopicId,
        CommentText,
        CommentedBy,
        CommentDateTime,
        IsDeleted,
        UserName
    FROM 
        Comments inner join Users on CommentedBy=userid
    WHERE 
        (@CommentId IS NULL OR CommentId = @CommentId)
        AND (@CommentedBy IS NULL OR CommentedBy = @CommentedBy)
        AND (@TopicId IS NULL OR TopicId = @TopicId)
    ORDER BY 
        CommentDateTime DESC;
END;

go
CREATE PROCEDURE sp_GetComment
    @CommentId INT=null
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CommentId,
        TopicId,
        CommentText,
        CommentedBy,
        CommentDateTime,
        IsDeleted
    FROM 
        Comments 
    WHERE 
        CommentId = @CommentId;
END;
