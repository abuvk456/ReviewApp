CREATE TABLE Votes (
    ID int PRIMARY KEY IDENTITY(1,1),
    VotedDate datetime NOT NULL,
    IsUpVote bit NOT NULL,
    VotedBy int NOT NULL,
    VotedFor int NOT NULL,
    FOREIGN KEY (VotedBy) REFERENCES Users(userid),
    FOREIGN KEY (VotedFor) REFERENCES Users(userid)
);
go

CREATE PROCEDURE sp_CreateVote
    @VotedDate DATETIME,
    @IsUpVote BIT,
    @VotedBy INT,
    @VotedFor INT,
    @ResultCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM Votes WHERE VotedBy = @VotedBy AND VotedFor = @VotedFor
    )
    BEGIN
        SET @ResultCode = 0; -- Failed: User has already voted for this topic
    END
    ELSE
    BEGIN
        INSERT INTO Votes (VotedDate, IsUpVote, VotedBy, VotedFor)
        VALUES (@VotedDate, @IsUpVote, @VotedBy, @VotedFor);

        -- Update user's upvote/downvote count
        IF @IsUpVote = 1
        BEGIN 
            UPDATE Users
            SET UpvoteCount = UpvoteCount + 1
            WHERE UserID = @VotedFor
        END
        ELSE
        BEGIN
            UPDATE Users
            SET DownvoteCount = DownvoteCount + 1
            WHERE UserID = @VotedFor
        END

        -- Add entry to Notifications table
        DECLARE @NotificationText NVARCHAR(500)
        DECLARE @TypeOfNotification NVARCHAR(50)
        DECLARE @FirstName NVARCHAR(50)
        DECLARE @LastName NVARCHAR(50)

        SELECT @FirstName = FirstName, @LastName = LastName FROM Users WHERE UserID = @VotedBy

        IF @IsUpVote = 1
        BEGIN
            SET @TypeOfNotification = 'Upvote'
            SET @NotificationText = CONCAT(@FirstName, ' ', @LastName, ' has voted you up.')
        END
        ELSE
        BEGIN
            SET @TypeOfNotification = 'Downvote'
            SET @NotificationText = CONCAT(@FirstName, ' ', @LastName, ' has voted you down.')
        END

        INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, IsRead, IsDeleted, RelatedDataID)
        VALUES (@TypeOfNotification, GETDATE(), @VotedBy, @VotedFor, @NotificationText, 0, 0, SCOPE_IDENTITY());

        SET @ResultCode = 1; -- Success: Vote created successfully
    END
END
GO



CREATE PROCEDURE sp_ReadVote
    @ID int = NULL,
    @VotedBy int = NULL,
    @VotedFor int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Votes
    WHERE (@ID IS NULL OR ID = @ID)
    AND (@VotedBy IS NULL OR VotedBy = @VotedBy)
    AND (@VotedFor IS NULL OR VotedFor = @VotedFor);
END
GO

CREATE PROCEDURE sp_UpdateVote
    @ID int,
    @VotedDate datetime,
    @IsUpVote bit,
    @VotedBy int,
    @VotedFor int
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Votes
    SET VotedDate = @VotedDate,
        IsUpVote = @IsUpVote,
        VotedBy = @VotedBy,
        VotedFor = @VotedFor
    WHERE ID = @ID;
END
GO

CREATE PROCEDURE sp_DeleteVote
    @ID int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Votes
    WHERE ID = @ID;
END
GO

