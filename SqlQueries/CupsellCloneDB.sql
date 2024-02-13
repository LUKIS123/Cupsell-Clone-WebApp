CREATE DATABASE CUPSELL_CLONE;

CREATE SCHEMA [users];
CREATE SCHEMA [products];
CREATE SCHEMA [orders];

--------------------------- USER -----------------------------
CREATE TABLE [users].[Roles]
(
    Id INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
);

CREATE TABLE [users].[Users]
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL,
    Username NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(50) NOT NULL,
    Name NVARCHAR(50) ,
    LastName NVARCHAR(50),
    DateOfBirth DATETIME2,
    Address NVARCHAR(200),
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES [users].[Roles] (Id),
    IsVerified BIT NOT NULL DEFAULT 0,
);

--------------------------- PRODUCT -----------------------------
/*
    Kategorie produktów
    (ENUM)
*/
CREATE TABLE [products].[ProductTypes]
(
    Id INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
);

/*
    Kategorie produktów
    (ENUM)
*/
CREATE TABLE [products].[Sizes]
(
    Id INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
);

/*
    Produkty oferowane przez Sklep => Koszulka, Kubek, id.
*/
CREATE TABLE [products].[Products]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Name NVARCHAR(200),
    Description NVARCHAR(200),
    ProductTypeId INT NOT NULL,
    FOREIGN KEY (ProductTypeId) REFERENCES [products].[ProductTypes] (Id),
);

/*
    Grafiki/Wzory zaprojektowane przez współpracujących sprzedających:
    Wzór jest własnością sprzedającego, do przechowywania grafik będzie później używana jakaś baza BLOBów
*/
CREATE TABLE [products].[Graphics]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    SellerId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (SellerId) REFERENCES [users].[Users] (Id),
);

/*
    Oferty wystawiane przez dany sklep są zestawieniem tabeli Produktów z tabelą Grafik.
    Są to oferty widoczne w sklepie.
*/

--TODO: MOZE TRZEBA BEDZIE DODAC TABELKI TYPU METODA PLATNOSCI, !!!!!STATUS PLATNOSCI!!!!! NA PEWNO
CREATE TABLE [products].[Offers]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES [products].[Products] (Id),
    GraphicsId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (GraphicsId) REFERENCES [products].[Graphics] (Id),
    SellerId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (SellerId) REFERENCES [users].[Users] (Id),
    Price DECIMAL(10, 2) NOT NULL,
    IsAvailable BIT NOT NULL,
);

/*
    AvaliableItems - Dostępna ilość przedmiotów w danych rozmiarach może być różna,
    dlatego ta tabela łączy Ofertę (dany rozmiar) oraz ilość na stanie [Quantity]. 
*/
CREATE TABLE [products].[AvailableItems]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    OfferId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (OfferId) REFERENCES [products].[Offers] (Id),
    SizeId INT NOT NULL,
    FOREIGN KEY (SizeId) REFERENCES [products].[Sizes] (Id),
    Quantity INT NOT NULL,
);

--------------------------- ORDER -----------------------------
CREATE TABLE [orders].[Status]
(
    Id INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
);

CREATE TABLE [orders].[Orders]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [users].[Users] (Id),
    OrderStatus INT NOT NULL,
    FOREIGN KEY (OrderStatus) REFERENCES [orders].[Status] (Id),
);

CREATE TABLE [orders].[OrderItems]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES [orders].[Orders] (Id),
    OfferId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (OfferId) REFERENCES [products].[Offers] (Id),
    SizeId INT NOT NULL,
    FOREIGN KEY (SizeId) REFERENCES [products].[Sizes] (Id),
    Quantity INT NOT NULL,
);

--------------------------- USER VERIFICATION -----------------------------

CREATE TABLE [users].[UserRefreshTokens]
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    RefreshToken NVARCHAR(1000) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [users].[Users] (Id),
);

CREATE TABLE [users].[UserVerificationTokens]
(
    UserId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [users].[Users] (Id),
    VerificationToken NVARCHAR(1000) NOT NULL,
);

---------------------------------
ALTER TABLE [users].[Users] 
ADD CONSTRAINT users_emails_unique UNIQUE (Email);

ALTER TABLE [users].[Users] 
ADD CONSTRAINT users_usernames_unique UNIQUE (Username);
