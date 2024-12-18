CREATE TABLE Sessions (
    SessionId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Token VARCHAR(255) NOT NULL,
    Expiration DATETIME NOT NULL,
    CreatedDate DATETIME NOT NULL,
    IsActive BIT NOT NULL,
    CONSTRAINT FK_Sessions_Users FOREIGN KEY (UserId) REFERENCES Users (UserId)
);
go

CREATE PROCEDURE sp_InsertSession
    @UserId INT,
    @Token VARCHAR(255),
    @Expiration DATETIME,
    @CreatedDate DATETIME,
    @IsActive BIT
AS
BEGIN
    INSERT INTO Sessions (UserId, Token, Expiration, CreatedDate, IsActive)
    VALUES (@UserId, @Token, @Expiration, @CreatedDate, @IsActive);
END
go

CREATE PROCEDURE sp_UpdateSession
    @SessionId INT,
    @UserId INT,
    @Token VARCHAR(255),
    @Expiration DATETIME,
    @CreatedDate DATETIME,
    @IsActive BIT
AS
BEGIN
    UPDATE Sessions
    SET UserId = @UserId,
        Token = @Token,
        Expiration = @Expiration,
        CreatedDate = @CreatedDate,
        IsActive = @IsActive
    WHERE SessionId = @SessionId;
END
go

CREATE PROCEDURE sp_DeleteSession
    @SessionId INT
AS
BEGIN
    DELETE FROM Sessions
    WHERE SessionId = @SessionId;
END
go
CREATE PROCEDURE sp_GetSessionById
    @SessionId INT
AS
BEGIN
    SELECT * FROM Sessions
    WHERE SessionId = @SessionId;
END

go

CREATE PROCEDURE sp_GetSessionByToken
    @Token VARCHAR(255)
AS
BEGIN
    SELECT * FROM Sessions
    WHERE Token = @Token;
END
go
