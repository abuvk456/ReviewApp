CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255)  NULL,
    HashPassword VARBINARY(64) NULL,
    Age INT NOT NULL,
    Country NVARCHAR(50) NOT NULL,
    Language NVARCHAR(50) NOT NULL,
    UpvoteCount INT NOT NULL CONSTRAINT DF_Users_UpvoteCount DEFAULT 0,
    DownvoteCount INT NOT NULL CONSTRAINT DF_Users_DownvoteCount DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_UpdatedDate DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    LastLoginDate DATETIME2,
    ModifiedBy int null,
    SecurityQuestion nvarchar(max),
    SecurityAnswer nvarchar(50)
    

);
go
Create PROCEDURE sp_InsertUser 
  @FirstName NVARCHAR(50),
  @LastName NVARCHAR(50),
  @Email NVARCHAR(255),
  @Password NVARCHAR(MAX),
  @Age INT,
  @Country NVARCHAR(50)='USA',
  @Language NVARCHAR(50)='English',
  @UpvoteCount INT=0,
  @DownvoteCount INT=0,
  @IsActive BIT
AS
BEGIN
  IF NOT EXISTS(SELECT 1 FROM Users WHERE Email = @Email)
  BEGIN
    DECLARE @HashedPassword VARBINARY(64) = CONVERT(VARBINARY(64), HASHBYTES('SHA2_256', @Password), 2)

    INSERT INTO Users (Username, FirstName, LastName, Email, HashPassword, Age, Country, Language, UpvoteCount, DownvoteCount, CreatedDate, UpdatedDate, IsActive, LastLoginDate)
    VALUES (@Email, @FirstName, @LastName, @Email, @HashedPassword, @Age, @Country, @Language, @UpvoteCount, @DownvoteCount, GETUTCDATE(), GETUTCDATE(), @IsActive, GETUTCDATE());
    
    SELECT SCOPE_IDENTITY();
  END
  ELSE
  BEGIN
    SELECT 0;
  END
END

go
-- CREATE PROCEDURE sp_for updating an existing user
CREATE PROCEDURE sp_UpdateUser
    @UserId INT,
    @Username NVARCHAR(50),
    @Email NVARCHAR(255),
    @Password NVARCHAR(MAX),
    @PasswordHash NVARCHAR(64),
    @Age INT,
    @Country NVARCHAR(50),
    @Language NVARCHAR(50),
    @UpvoteCount INT,
    @DownvoteCount INT,
    @IsActive BIT,
    @LastLoginDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Hash the password using SHA2_256 algorithm
    SET @PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @Password), 2);

    UPDATE Users
    SET 
        Username = @Username,
        Email = @Email,
        Password = @PasswordHash,
        Age = @Age,
        Country = @Country,
        Language = @Language,
        UpvoteCount = @UpvoteCount,
        DownvoteCount = @DownvoteCount,
        IsActive = @IsActive,
        LastLoginDate = @LastLoginDate,
        UpdatedDate = GETUTCDATE()
    WHERE UserId = @UserId;
END;

GO
CREATE PROCEDURE sp_ChangePassword
  @UserId INT,
  @OldPassword NVARCHAR(255),
  @NewPassword NVARCHAR(255),
  @RowsAffected INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @CurrentPassword NVARCHAR(255);

  SELECT @CurrentPassword = Password
  FROM Users
  WHERE UserId = @UserId;

  IF HASHBYTES('SHA2_256', @OldPassword) = @CurrentPassword
  BEGIN
    UPDATE Users
    SET Password = HASHBYTES('SHA2_256', @NewPassword),
        UpdatedDate = GETUTCDATE()
    WHERE UserId = @UserId;

    SET @RowsAffected = @@ROWCOUNT;

    SELECT @RowsAffected;
  END
  ELSE
  BEGIN
    SET @RowsAffected = 0;

    SELECT @RowsAffected;
  END
END;
go
-- CREATE PROCEDURE sp_for deleting a user by ID
CREATE PROCEDURE sp_DeleteUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Users
    WHERE UserId = @UserId;
END;
go

CREATE PROCEDURE [dbo].[sp_SignInUser]
    @Email nvarchar(255),
    @Password nvarchar(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HashedPassword nvarchar(255);

    -- Get the hashed password for the user
    SELECT @HashedPassword = HashPassword
    FROM Users
    WHERE Email = @Email;

    -- Check if the provided password matches the hashed password
    IF @HashedPassword IS NOT NULL AND HASHBYTES('SHA2_256', @Password) = @HashedPassword
    BEGIN
        -- Sign-in successful, return the user row
        SELECT *
        FROM Users
        WHERE Email = @Email;
    END
    ELSE
    BEGIN
        -- Sign-in failed, return empty result set
        SELECT NULL
        WHERE 1 = 0;
    END
END
go

CREATE PROCEDURE [dbo].[sp_GetUsers]
    @UserId int = null,
    @Username nvarchar(50) = null,
    @Email nvarchar(255) = null,
    @FirstName nvarchar(50) = null,
    @LastName nvarchar(50) = null,
    @SearchTerm nvarchar(50) = null
AS
BEGIN
    SET NOCOUNT ON;

    
SELECT [UserId]
      ,[Username]
      ,[FirstName]
      ,[LastName]
      ,[Email]
      ,[Age]
      ,[Country]
      ,[Language]
      ,[UpvoteCount]
      ,[DownvoteCount]
      ,[CreatedDate]
      ,[IsActive]
      ,[LastLoginDate]
  FROM [dbo].[Users]
    WHERE (@UserId IS NULL OR UserId = @UserId)
        AND (@Username IS NULL OR Username = @Username)
        AND (@Email IS NULL OR Email = @Email)
        AND (@FirstName IS NULL OR FirstName = @FirstName)
        AND (@LastName IS NULL OR LastName = @LastName)
        AND (@SearchTerm IS NULL OR FirstName LIKE '%' + @SearchTerm + '%' OR LastName LIKE '%' + @SearchTerm + '%')
END


--exec sp_getuserStats @userid=2
go
create PROCEDURE sp_GetUserStats
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TopicsCount INT;
    DECLARE @FollowingsCount INT;
    DECLARE @FollowersCount INT;
    DECLARE @WatchListCount INT;

    SELECT @TopicsCount = COUNT(*) FROM Topics WHERE CreatedBy = @UserId;
    SELECT @FollowingsCount = COUNT(*) FROM Followings WHERE FollowedBy = @UserId;
    SELECT @FollowersCount = COUNT(*) FROM Followings WHERE FollowedUserId = @UserId;
    SELECT @WatchListCount = COUNT(*) FROM Watchlist WHERE UserID = @UserId;

    SELECT @TopicsCount AS TopicsCount, @FollowingsCount AS FollowingsCount, @FollowersCount AS FollowersCount, @WatchListCount AS WatchListCount;
END
GO
create PROCEDURE [dbo].[sp_GetUsersSameWatchList]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserTopicCount INT;

    -- Get the total count of topics in the requested user's watchlist
    SELECT @UserTopicCount = COUNT(DISTINCT TopicID)
    FROM [dbo].[Watchlist]
    WHERE UserID = @UserId;

    SELECT DISTINCT U.[UserId]
        ,U.[Username]
        ,U.[FirstName]
        ,U.[LastName]
        ,U.[Email]
        ,U.[Age]
        ,U.[Country]
        ,U.[Language]
        ,U.[UpvoteCount]
        ,U.[DownvoteCount]
        ,U.[CreatedDate]
        ,U.[IsActive]
        ,U.[LastLoginDate]
    FROM [dbo].[Users] U
    INNER JOIN [dbo].[Watchlist] W ON W.[UserID] = U.[UserId]
    WHERE U.UserId <> @UserId
        AND U.IsActive = 1 -- Optional: To filter active users only
        AND EXISTS (
            SELECT 1
            FROM [dbo].[Watchlist] W2
            WHERE W2.UserID = U.UserID 
            GROUP BY W2.UserID
            HAVING COUNT(DISTINCT W2.TopicID) >= (@UserTopicCount * 1)
        )
END
go
CREATE PROCEDURE [dbo].[sp_ResetPassword]
    @Email nvarchar(255),
    @NewPassword nvarchar(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Update the user's password
    UPDATE Users
    SET HashPassword = HASHBYTES('SHA2_256', @NewPassword)
    WHERE Email = @Email;

    -- Check if any rows were affected by the update
    IF @@ROWCOUNT > 0
    BEGIN
        -- Password reset successful, return success status
        SELECT 'Password reset successful.' AS Result;
    END
    ELSE
    BEGIN
        -- Password reset failed, return failure status
        SELECT 'Failed to reset password. User not found.' AS Result;
    END
END
