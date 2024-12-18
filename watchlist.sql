CREATE TABLE Watchlist (
    UserID int NOT NULL,
    TopicID int NOT NULL,
    WatchedDateTime datetime NOT NULL,
    CONSTRAINT PK_Watchlist PRIMARY KEY (UserID, TopicID),
    CONSTRAINT FK_Watchlist_User FOREIGN KEY (UserID) REFERENCES Users(UserId),
    CONSTRAINT FK_Watchlist_Topic FOREIGN KEY (TopicID) REFERENCES Topics(topicid)
);
GO

CREATE PROCEDURE InsertWatchlist
    @UserID int,
    @TopicID int,
    @WatchedDateTime datetime
AS
BEGIN
    INSERT INTO Watchlist (UserID, TopicID, WatchedDateTime)
    VALUES (@UserID, @TopicID, @WatchedDateTime);
END
GO

CREATE PROCEDURE UpdateWatchlist
    @UserID int,
    @TopicID int,
    @WatchedDateTime datetime
AS
BEGIN
    UPDATE Watchlist
    SET WatchedDateTime = @WatchedDateTime
    WHERE UserID = @UserID AND TopicID = @TopicID;
END
GO

CREATE PROCEDURE DeleteWatchlist
    @UserID int,
    @TopicID int
AS
BEGIN
    DELETE FROM Watchlist
    WHERE UserID = @UserID AND TopicID = @TopicID;
END
GO

CREATE PROCEDURE SelectWatchlistByUserID2
    @UserID int
AS
BEGIN
    SELECT w.UserID, w.TopicID, t.Title, w.WatchedDateTime
    FROM Watchlist w
    INNER JOIN Topics t ON w.TopicID = t.topicid
    WHERE w.UserID = @UserID
    ORDER BY w.WatchedDateTime DESC;
END
GO



    
    create PROCEDURE SelectWatchlistByUserID
    @UserID int
AS
BEGIN
    SELECT w.UserID as CreatedBy, w.TopicId, 
	t.Title, 
    t.Description,
    t.TopicType, 
    t.TopicImage,
    t.TopicVideo,
    t.CreatedBy,
    t.CreatedDate, 
	0 as IDMBID , 
	0 as TMDBID, 
    t.IsActive, 
    t.IMDBRating, '' as Genre, w.WatchedDateTime
    FROM Watchlist w
    INNER JOIN Topics t ON w.TopicID = t.topicid
    WHERE w.UserID = @UserID
    ORDER BY w.WatchedDateTime DESC;
END
GO

