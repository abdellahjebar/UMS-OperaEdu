SET QUOTED_IDENTIFIER ON;
UPDATE Users 
SET PasswordHash = '$2a$11$35MRZqmRvQtU486SJxS2Nu2FTg6oU0cTV.zs3kKNKjHWSEEyERJB2' 
WHERE Email = 'admin@testuniversity.edu';

SELECT Email, PasswordHash, UserType 
FROM Users 
WHERE Email = 'admin@testuniversity.edu';
