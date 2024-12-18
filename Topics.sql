/* Topics SP */
CREATE TABLE Topics (
    TopicId INT PRIMARY KEY IDENTITY(1,1),
    Title nvarchar(255) NOT NULL,
    Description nvarchar(MAX) NOT NULL,
    TopicType nvarchar(50) NOT NULL,
    TopicImage nvarchar(255),
    TopicVideo nvarchar(255),
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2(7) DEFAULT(GETUTCDATE()) NOT NULL,
    IsActive BIT DEFAULT(1) NOT NULL,
    IMDBRating int default(0) null
    CONSTRAINT FK_Topics_Users FOREIGN KEY (CreatedBy) REFERENCES Users (userid)
);

GO
CREATE PROCEDURE sp_SelectTopics
    @TopicId INT = NULL,
    @TopicType nvarchar(50) = NULL,
    @CreatedBy INT = NULL,
    @SearchTerm nvarchar(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

   SELECT 
        TopicId,
        Title,
        Description,
        TopicType,
        TopicImage,
        TopicVideo,
        IMDBRating,
        CreatedBy,
        Topics.CreatedDate,
        topics.IsActive,
        UserName,
		email,
		FirstName+' ' + LastName as Name
    FROM 
        Topics inner join Users on createdby=userid
    WHERE 
        (@TopicId IS NULL OR TopicId = @TopicId) AND
        (@TopicType IS NULL OR TopicType = @TopicType) AND
        (@CreatedBy IS NULL OR CreatedBy = @CreatedBy) AND
        (@SearchTerm IS NULL OR (Title LIKE '%' + @SearchTerm + '%' OR Description LIKE '%' + @SearchTerm + '%'));
END;
GO
CREATE PROCEDURE sp_InsertTopic
    @Title nvarchar(255),
    @Description nvarchar(MAX),
    @TopicType nvarchar(50) = 'Movie',
    @TopicImage nvarchar(255) = NULL,
    @TopicVideo nvarchar(255) = NULL,
    @IMDBRating INT = NULL,
    @CreatedBy INT = 2,
    @NewTopicId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Topics (Title, Description, TopicType, TopicImage, TopicVideo, IMDBRating, CreatedBy)
    VALUES (@Title, @Description, @TopicType, @TopicImage, @TopicVideo, @IMDBRating, @CreatedBy);

    SET @NewTopicId = SCOPE_IDENTITY();
END;

GO

CREATE Procedure sp_UpdateTopic
    @TopicId INT,
    @Title nvarchar(255),
    @Description nvarchar(MAX),
    @TopicType nvarchar(50),
    @TopicImage nvarchar(255),
    @TopicVideo nvarchar(255),
    @IMDBRating INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Topics 
    SET Title = @Title, Description = @Description, TopicType = @TopicType, TopicImage = @TopicImage, TopicVideo = @TopicVideo, IMDBRating = @IMDBRating
    WHERE TopicId = @TopicId;
END;
go

CREATE Procedure sp_DeleteTopic
    @TopicId INT
    
AS
BEGIN
    SET NOCOUNT ON;
	delete Watchlist where  topicid=@TopicId
	delete Comments where topicid=@TopicId
    delete Topics where topicid=@TopicId
END;
go
