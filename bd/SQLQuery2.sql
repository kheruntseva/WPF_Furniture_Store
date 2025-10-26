DROP DATABASE MyWpfAppDb;
CREATE DATABASE MyWpfAppDb;

CREATE TABLE RectangleItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Price DECIMAL(18,2) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    Image NVARCHAR(300)
);

CREATE TABLE ItemTranslations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RectangleItemId UNIQUEIDENTIFIER NOT NULL,
    LanguageCode NVARCHAR(2) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Category NVARCHAR(50),
    Availability NVARCHAR(50),
    FOREIGN KEY (RectangleItemId) REFERENCES RectangleItems(Id)
);

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    IsAdmin BIT NOT NULL DEFAULT 0
);

ALTER TABLE Users
ADD Password NVARCHAR(255) NOT NULL DEFAULT '';

CREATE TABLE PurchasedProducts (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RectangleItemId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (UserId, RectangleItemId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RectangleItemId) REFERENCES RectangleItems(Id)
);

CREATE TABLE OrderHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RectangleItemId UNIQUEIDENTIFIER NOT NULL,
    PurchaseDate DATETIME NOT NULL DEFAULT GETDATE(),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    Category NVARCHAR(50) NULL,
    Availability NVARCHAR(50) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RectangleItemId) REFERENCES RectangleItems(Id)
);

SELECT * FROM RectangleItems;
SELECT * FROM ItemTranslations;
SELECT * FROM Users;
SELECT * FROM PurchasedProducts;

INSERT INTO RectangleItems (Id, Price, Color, Image)
VALUES 
    ('123e4567-e89b-12d3-a456-426614174000', 150.00, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic2.1.webp'),
    ('223e4567-e89b-12d3-a456-426614174001', 170.00, 'Wood', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic5.1.webp');

-- Insert into ItemTranslations
INSERT INTO ItemTranslations (RectangleItemId, LanguageCode, Name, Description, Category, Availability)
VALUES 
    ('123e4567-e89b-12d3-a456-426614174000', 'ru', '�������� ����', '�������� ���� �������� �������, �������� �������� ��� ��������� �����. ��� ����������� ������ �������� � ���� ���������������� � �������, �������� ������ ��������� � �����.', '�����', '� �������'),
    ('123e4567-e89b-12d3-a456-426614174000', 'en', 'Kitchen Table', 'The kitchen table is medium-sized, ideal for a small family. Its modern design combines functionality and comfort, creating a cozy atmosphere in the kitchen.', 'Kitchen', 'In stock'),
    ('223e4567-e89b-12d3-a456-426614174001', 'ru', '�����', '"���� �������� � �������������� ����� � 6 ���������� ������� ������� ���� �������. ����������� �� �������������� ������, �� ���������� ���������� �������� � ������� ������� � ������������� ��������, ������� �������� ��� ��������.', '�������', '� �������'),
    ('223e4567-e89b-12d3-a456-426614174001', 'en', 'Drawer Dresser', 'Elevate your bedroom with this stylish and functional 6-drawer dresser. Crafted from engineered wood, it boasts a sleek, clean-lined design available in a variety of finishes to complement your decor.', 'Bedroom', 'In stock');

	UPDATE RectangleItems
SET Price = 150
WHERE Id = '123e4567-e89b-12d3-a456-426614174000';

UPDATE RectangleItems
SET Price = 170
WHERE Id = '223e4567-e89b-12d3-a456-426614174001';

-- Insert into RectangleItems
INSERT INTO RectangleItems (Id, Price, Color, Image)
VALUES 
    ('123e4567-e89b-12d3-a456-426614174002', 320, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic3.1.webp'),
    ('223e4567-e89b-12d3-a456-426614174003', 90, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic8.1.webp');

-- Insert into ItemTranslations
INSERT INTO ItemTranslations (RectangleItemId, LanguageCode, Name, Description, Category, Availability)
VALUES 
    ('123e4567-e89b-12d3-a456-426614174002', 'ru', '��������� ����', '�������� �������, ����� �������� ���� �����. ������������� ���� � ����������� �������� �������� ��� ��������������� ������������', '�����', '� �������'),
    ('123e4567-e89b-12d3-a456-426614174002', 'en', 'Kitchen cabinet', 'a great solution to decorate your kitchen. A roomy cabinet with transparent doors is suitable for a minimalistic space.', 'Kitchen', 'In order'),
    ('223e4567-e89b-12d3-a456-426614174003', 'ru', '������� �������', '��� ������� ������������� ������� � ����������� � �������� ����� �������� �������� ��� ������ ��������� � ������ ������. ������ ������� ��������� ���������� ����� ��� 60 ��������� ���������.', '�������', '� �������'),
    ('223e4567-e89b-12d3-a456-426614174003', 'en', 'round-shaped mirror', 'This modern and chic style metal round mirror matches any room and any decor perfectly. Every single mirror is carefully crafted with over 60 processing on one item.', 'Bedroom', 'In order');

-- Insert into RectangleItems
INSERT INTO RectangleItems (Id, Price, Color, Image)
VALUES 
    ('a1b2c3d4-e5f6-7890-abcd-123456789001', 340, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic6.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789002', 90, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic7.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789003', 190, 'Wood', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic4.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789004', 70, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic9.1.webp');

-- Insert into ItemTranslations
INSERT INTO ItemTranslations (RectangleItemId, LanguageCode, Name, Description, Category, Availability)
VALUES 
    ('a1b2c3d4-e5f6-7890-abcd-123456789001', 'ru', '��������� ������', '��������� ������ �������-������ �� ������ � ���������. ��� ��������� ��������� ���������������� � ����� ��� ������ ����.', '������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789001', 'en', 'Double Bathroom Vanity', 'Vanity Art introducing the premium poplar wood bath vanity with sink. It is the perfect blend of functionality and style for your home.', 'Bathroom', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789002', 'ru', '������� �������������', '��������� �������� ������ ���� ���� ����������� ������������� ��������. ��� ������� � ����� ����������� �� ������������ ������ � �������� ���������� ����������� (���������� �������� / �����������), ������� �������� ������� ������������� � ��������, ����������� ����� ��� ����.', '������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789002', 'en', 'Rectangle bathroom mirror', 'Complete your house decor with this modern contemporary rectangle mirror. This framed mirror is crafted with aluminum alloy frame with excellent surface process(powder coated/brushed/frosted), which is highly resistant to corrosion, moist or water.', 'Bathroom', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789003', 'ru', '��������� ������ ������', '���� �� �������� ����������������� ��������� ����, ���� ������� ������ ����� ��� ���. ����������� �� �������� ������, ��������� �� ������� ������ �����������', '�����', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789003', 'en', 'wall mounted wine rack', 'If youre a vintage wine collector, this piece of furniture is definitely for you. made of durable wood, the stand will not leave anyone indifferent.', 'Kitchen', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789004', 'ru', '������� ��� ���������', '��� ��������� ����� ��� ������ ������� - �������� ������ ������������ ��� �����������. ��� �������� ����� ��������� �������, ������� ������������ ���������� ����� ��� ���������, ������� �� ������ ����� ��� �����.', '������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789004', 'en', 'Wall Mounted Bathroom Shelves', 'These wall-mounted bathroom shelves offer a stylish way to organize your essentials.They feature three mesh shelves that provide ample space for items you want on hand. ', 'Bathroom', 'In order');


	-- Insert into RectangleItems
INSERT INTO RectangleItems (Id, Price, Color, Image)
VALUES 
    ('a1b2c3d4-e5f6-7890-abcd-123456789005', 60, 'White', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic10.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789006', 90, 'white', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic11.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789007', 120, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic12.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789008', 340, 'White', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic13.1.webp');

-- Insert into ItemTranslations
INSERT INTO ItemTranslations (RectangleItemId, LanguageCode, Name, Description, Category, Availability)
VALUES 
    ('a1b2c3d4-e5f6-7890-abcd-123456789005', 'ru', '�������� ����', '����������� ����� ���� � ����������� ������� � ���������� ��������. ������ � ������� ����������� �������� �������� � �������� ����� ��� ��������. ������ ������� ������������ �������, � ������������� ���� ���������� � ����� �������.', '�����', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789005', 'en', 'kitchen chair', 'A modern white chair with wooden legs and a simple design. The lightweight and durable construction fits perfectly into the interior of a kitchen or dining room. The padded seat provides comfort, and the versatile color can be combined with any decor.', 'Kitchen', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789006', 'ru', '�������� ����', '������������ ����� ������ ��������� ������ ����, ����� ������� �������. ������, ������������ � ������� � ����� � ������� ��� ��������� ������������� �� ����� ��� � ����. ����� ������ � ����������� ���������!', '�����', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789006', 'en', 'Kitchen chair', 'The ergonomic shape of the backrest follows the curves of the body, making the fit comfortable. Lightweight, moisture�resistant and easy to care for - ideal for active use in the kitchen or in cafes. A bright accent in a modern interior!', 'Kitchen', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789007', 'ru', '���������� ��� ��������� �������', '���������� ��������� ���������� ������� ���� � ��� �������� � ��������� ������ ������ �� �����', '�����', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789007', 'en', 'kitchen island lamp', 'The compact pendant lamp will add comfort to your interior and provide warm evenings in the kitchen.', 'Kitchen', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789008', 'ru', '������� � ��������� �����', '������� � ��������� ����� � ��� ������ � ����������� ��������  �� ��������� ��������� �����. ����� ������ ������� ������ ����� ��������� � �������� ���. ����� ������������ � ������������ � ����������� ���������� ��� �������� ������� ������������ �������������. �������� ��� ���, ��� ����� ��������� ���������������� � �������.', '�������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456789008', 'en', 'a bed with a flanged edge', 'A bed with a flanged edge is a model with a protruding side around the perimeter of the sleeping place. This design gives the furniture a more prominent and stylish look. It is often used in classic and modern interiors to give the bed an aesthetic finish. Suitable for those who appreciate the combination of functionality and design.', 'Bedroom', 'In order');


		-- Insert into RectangleItems
INSERT INTO RectangleItems (Id, Price, Color, Image)
VALUES 
    ('a1b2c3d4-e5f6-7890-abcd-123456788001', 170, 'Wood', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic15.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788002', 350, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic16.1.webp'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788004', 350, 'Black', 'D:\\������� 4 ���\\���\\��� 4-5\\mywpf\\bin\\Debug\\Images\\pic14.1.webp');

-- Insert into ItemTranslations
INSERT INTO ItemTranslations (RectangleItemId, LanguageCode, Name, Description, Category, Availability)
VALUES 
    ('a1b2c3d4-e5f6-7890-abcd-123456788001', 'ru', '������ ���� �� �������', '�������� ���������� ���� � �������� ��������, �� ����� ��� ����� ����� ��� �������� ����� �������, ��� �������� � �������, �������� ������ ������, ��������� �������������� � ������ ������.', '������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788001', 'en', 'bamboo bathroom shelving', 'Beautiful bamboo cabinet with a pleasing design, it has three tiers of shelves for storage needed for little things like health and beauty, first aid, toilet tissue, and much more. ', 'Bathroom', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788002', 'ru', '������� ������ ����������', '������� ���������� ������: ����� ������ �� ����������� ������ �������� 5/16 �����, �������������� �������������� ��������� � ������������ ��� ���������� ���������� ������ ������������.', '������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788002', 'en', 'Tempered Glass Frameless Sliding Shower', 'Durable Tempered Glass: Features 5/16 inch thick tempered glass panels, providing exceptional strength and safety while maintaining crystal-clear transparency.', 'Bathroom', 'In order'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788004', 'ru', '������� �� ����������� �������', ' ������ ������ ������� �������� ���������� ������ ������ � ������ � �������, �������� ��������� ����������� � �������� � �������. ���������� �������� ������ � ������ ������� ������� ������� ������ ���� � ��������� �����������������, ����� �� �� ������ �������������� ��������� ������, �� � ������������� ���������, ������� ��������� � ���� ������������ ������� � ������� ��������� �����.', '�������', '� �������'),
    ('a1b2c3d4-e5f6-7890-abcd-123456788004', 'en', 'solid wood bed', 'Every detail of the bed reflects the craftsmanship and care of the details, creating an atmosphere of tranquility and comfort in the bedroom. The unique wood texture and warm shades give the bed a special charm and natural appeal, making it not only a functional piece of furniture, but also a work of art that embodies the natural beauty and warmth of the hearth', 'Bedroom', 'In order');

ALTER TABLE Users
DROP COLUMN IsAdmin;
ALTER TABLE Users
ADD Password NVARCHAR(100) NOT NULL DEFAULT '';
ALTER TABLE Users
ADD CONSTRAINT UQ_Users_Username UNIQUE (Username);

INSERT INTO users(Username, IsAdmin, Id, Password)
Values
('Daria', '0', 'a1b2c3d4-e5f6-7890-abcd-123456788004', '0725');

UPDATE ItemTranslations 
SET Name='bamboo bathroom shelving',
 Description = 'Beautiful bamboo cabinet with a pleasing design, it has three tiers of shelves for storage needed for little things like health and beauty, first aid, toilet tissue, and much more. ',
 Category='Bathroom'
WHERE RectangleItemId = 'a1b2c3d4-e5f6-7890-abcd-123456788001' and LanguageCode ='en';

UPDATE ItemTranslations 
SET Availability='in order'
WHERE RectangleItemId = 'a1b2c3d4-e5f6-7890-abcd-123456788001' and LanguageCode ='en';

UPDATE ItemTranslations 
SET Name='Tempered Glass Frameless Sliding Shower',
 Description = 'Durable Tempered Glass: Features 5/16 inch thick tempered glass panels, providing exceptional strength and safety while maintaining crystal-clear transparency.',
 Category='Bathroom',
 Availability='in order'
WHERE RectangleItemId = 'a1b2c3d4-e5f6-7890-abcd-123456788002' and LanguageCode ='en';

UPDATE ItemTranslations 
SET Name='solid wood bed',
 Description = 'Every detail of the bed reflects the craftsmanship and care of the details, creating an atmosphere of tranquility and comfort in the bedroom. The unique wood texture and warm shades give the bed a special charm and natural appeal, making it not only a functional piece of furniture, but also a work of art that embodies the natural beauty and warmth of the hearth',
 Category='Bedroom',
 Availability='in order'
WHERE RectangleItemId = 'a1b2c3d4-e5f6-7890-abcd-123456788004' and LanguageCode ='en';

CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RectangleItemId UNIQUEIDENTIFIER NOT NULL,
    ReviewText NVARCHAR(MAX) NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RectangleItemId) REFERENCES RectangleItems(Id)
);

UPDATE Users SET Password = 'admin' WHERE Username = 'admin';

SELECT * FROM Reviews;
SELECT * FROM PurchasedProducts;

UPDATE ItemTranslations 
SET Name='������ ���� �� �������',
 Description = '�������� ���������� ���� � �������� ��������, �� ����� ��� ����� ����� ��� �������� ����� �������, ��� �������� � �������, �������� ������ ������, ��������� �������������� � ������ ������. ',
 Category='������'
WHERE RectangleItemId = 'a1b2c3d4-e5f6-7890-abcd-123456788001' and LanguageCode ='ru';

UPDATE ItemTranslations 
SET Availability='in order'
WHERE RectangleItemId = 'a1b2c3d4-e5f6-7890-abcd-123456788001' and LanguageCode ='en';
