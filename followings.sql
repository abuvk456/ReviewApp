CREATE TABLE Followings (
    FollowedUserId int NOT NULL,
    DatedStarted datetime NOT NULL,
    FollowedBy int NOT NULL,
    PRIMARY KEY (FollowedUserId, FollowedBy),
    FOREIGN KEY (FollowedUserId) REFERENCES Users(Userid),
    FOREIGN KEY (FollowedBy) REFERENCES Users(UserID)
);
go
create PROCEDURE [dbo].[CreateFollowing]
    @FollowedUserId INT,
    @FollowedBy INT,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @FollowedUserId < 0
    BEGIN
        -- Convert negative FollowedUserId to positive and delete the entry
        SET @FollowedUserId = ABS(@FollowedUserId);

        -- Retrieve the username of the user being unfollowed
        DECLARE @UnfollowedUsername NVARCHAR(100);
        SELECT @UnfollowedUsername = Username FROM Users WHERE UserID = @FollowedUserId;
		 DECLARE @FollowedbyUsername NVARCHAR(100);
        SELECT @FollowedbyUsername = Username FROM Users WHERE UserID = @FollowedBy;

        -- Delete the entry from the Followings table
        DELETE FROM Followings
        WHERE FollowedUserId = @FollowedUserId AND FollowedBy = @FollowedBy;

        -- Create a notification for the user being unfollowed
        DECLARE @UnfollowedNotificationText NVARCHAR(500);
        SET @UnfollowedNotificationText = CONCAT('User : ', @FollowedbyUsername, ' stopped following you.');

        INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, RelatedDataId)
        VALUES ('Unfollowed', GETDATE(), @FollowedBy, @FollowedUserId, @UnfollowedNotificationText, NULL);

        -- Create a notification for the user who initiated the unfollow
        DECLARE @UnfollowNotificationText NVARCHAR(500);
        SET @UnfollowNotificationText = CONCAT('You stopped following user: ', @UnfollowedUsername, '.');

        INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, RelatedDataId)
        VALUES ('Unfollow', GETDATE(), @FollowedBy, @FollowedBy, @UnfollowNotificationText, NULL);

        SET @RowsAffected = @@ROWCOUNT;
    END
    ELSE IF NOT EXISTS (SELECT 1 FROM Followings WHERE FollowedUserId = @FollowedUserId AND FollowedBy = @FollowedBy)
    BEGIN
        -- Follow action
        INSERT INTO Followings (FollowedUserId, DatedStarted, FollowedBy)
        VALUES (@FollowedUserId, GETDATE(), @FollowedBy);

        DECLARE @FollowedNotificationText NVARCHAR(500);
        SET @FollowedNotificationText = CONCAT('User: ', @FollowedbyUsername, ' started following you.');

        INSERT INTO Notifications (TypeOfNotification, CreatedDatetime, CreatedBy, CreatedFor, NotificationText, RelatedDataId)
        VALUES ('New Follower', GETDATE(), @FollowedBy, @FollowedUserId, @FollowedNotificationText, SCOPE_IDENTITY());

        SET @RowsAffected = @@ROWCOUNT;
    END
    ELSE
    BEGIN
        SET @RowsAffected = 0;
    END
END
GO

go
CREATE PROCEDURE ReadFollowing
    @FollowedUserId int = NULL,
    @FollowedBy int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT users.*
    FROM users
    JOIN followings ON users.userid = followings.FollowedBy
    WHERE Followings.FollowedUserId = @FollowedBy
END
GO


CREATE PROCEDURE UpdateFollowing
    @FollowedUserId int,
    @FollowedBy int,
    @DatedStarted datetime
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Followings
    SET DatedStarted = @DatedStarted
    WHERE FollowedUserId = @FollowedUserId
    AND FollowedBy = @FollowedBy;
END
GO

CREATE PROCEDURE DeleteFollowing
    @FollowedUserId int,
    @FollowedBy int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Followings
    WHERE FollowedUserId = @FollowedUserId
    AND FollowedBy = @FollowedBy;
END
GO

