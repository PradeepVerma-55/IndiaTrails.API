
## Entity: Region

| Column       | Type            | Constraints | Description                             |
| ------------ | --------------- | ----------- | --------------------------------------- |
| `Id`         | `Guid`          | Primary Key | Unique identifier                       |
| `Code`       | `nvarchar(3)`   | Not Null    | Short code for the region (e.g., “AKL”) |
| `Name`       | `nvarchar(100)` | Not Null    | Full name of the region                 |
| `Area`       | `double`        |             | Total area in square kilometers         |
| `Lat`        | `double`        |             | Latitude                                |
| `Long`       | `double`        |             | Longitude                               |
| `Population` | `int`           |             | Population count                        |


## Entity: Walk

| Column             | Type            | Constraints                        | Description                      |
| ------------------ | --------------- | ---------------------------------- | -------------------------------- |
| `Id`               | `Guid`          | Primary Key                        | Unique identifier                |
| `Name`             | `nvarchar(100)` | Not Null                           | Name of the walk                 |
| `Description`      | `nvarchar(500)` |                                    | Description of the track         |
| `LengthInKm`       | `double`        | Not Null                           | Length of the walk in kilometers |
| `RegionId`         | `Guid`          | Foreign Key → `Region(Id)`         | The region the walk belongs to   |
| `WalkDifficultyId` | `Guid`          | Foreign Key → `WalkDifficulty(Id)` | Difficulty level of the walk     |


## Entity: WalkDifficulty

| Column | Type           | Constraints | Description                                       |
| ------ | -------------- | ----------- | ------------------------------------------------- |
| `Id`   | `Guid`         | Primary Key | Unique identifier                                 |
| `Code` | `nvarchar(20)` | Not Null    | Difficulty level (e.g., “Easy”, “Medium”, “Hard”) |

## Create the initial migration
  Add-Migration InitialCreate

## Update the database to apply the migration
  Update-Database



## Add data in case you needed.
USE [IndiaTrailsDB]
GO

-- Clean existing data
DELETE FROM [IndiaTrailsDB].[dbo].[Walks];
DELETE FROM [IndiaTrailsDB].[dbo].[Regions];
DELETE FROM [IndiaTrailsDB].[dbo].[Difficulties];
GO

-- Seed Difficulties
INSERT [dbo].[Difficulties] ([Id], [Name]) VALUES (N'f808ddcd-b5e5-4d80-b732-1ca523e48434', N'Hard');
INSERT [dbo].[Difficulties] ([Id], [Name]) VALUES (N'54466f17-02af-48e7-8ed3-5a4a8bfacf6f', N'Easy');
INSERT [dbo].[Difficulties] ([Id], [Name]) VALUES (N'ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c', N'Medium');
GO

-- Seed Indian Regions
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'1a111111-1111-1111-1111-111111111111', N'HP', N'Himachal Pradesh', N'https://images.pexels.com/photos/674010/pexels-photo-674010.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'2b222222-2222-2222-2222-222222222222', N'UK', N'Uttarakhand', N'https://images.pexels.com/photos/5334653/pexels-photo-5334653.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'3c333333-3333-3333-3333-333333333333', N'LD', N'Ladakh', N'https://images.pexels.com/photos/1566435/pexels-photo-1566435.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'4d444444-4444-4444-4444-444444444444', N'SK', N'Sikkim', N'https://images.pexels.com/photos/1547613/pexels-photo-1547613.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'5e555555-5555-5555-5555-555555555555', N'AR', N'Arunachal Pradesh', N'https://images.pexels.com/photos/1868778/pexels-photo-1868778.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'6f666666-6666-6666-6666-666666666666', N'KL', N'Kerala', N'https://images.pexels.com/photos/248062/pexels-photo-248062.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
INSERT [dbo].[Regions] ([Id], [Code], [Name], [RegionImageUrl]) VALUES (N'7g777777-7777-7777-7777-777777777777', N'GA', N'Goa', N'https://images.pexels.com/photos/372281/pexels-photo-372281.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1');
GO

-- Seed Walks (India)
INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('10111111-aaaa-bbbb-cccc-111111111111', 'Triund Trek', 'A popular beginner-friendly trek in Himachal Pradesh with panoramic views of the Dhauladhar range.', 9.0,
'https://images.pexels.com/photos/674010/pexels-photo-674010.jpeg', 'ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c', '1a111111-1111-1111-1111-111111111111');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('20222222-aaaa-bbbb-cccc-222222222222', 'Hampta Pass Trek', 'A crossover trek between Kullu and Lahaul valleys offering contrasting landscapes.', 26.0,
'https://images.pexels.com/photos/674010/pexels-photo-674010.jpeg', 'f808ddcd-b5e5-4d80-b732-1ca523e48434', '1a111111-1111-1111-1111-111111111111');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('30333333-aaaa-bbbb-cccc-333333333333', 'Valley of Flowers Trek', 'A UNESCO World Heritage site, famous for its vibrant alpine flora and scenic landscapes.', 17.0,
'https://images.pexels.com/photos/5334653/pexels-photo-5334653.jpeg', 'ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c', '2b222222-2222-2222-2222-222222222222');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('40444444-aaaa-bbbb-cccc-444444444444', 'Kedarkantha Trek', 'A classic winter trek with snow-clad peaks and dense pine forests.', 20.0,
'https://images.pexels.com/photos/5334653/pexels-photo-5334653.jpeg', 'ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c', '2b222222-2222-2222-2222-222222222222');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('50555555-aaaa-bbbb-cccc-555555555555', 'Chadar Trek', 'A unique frozen river trek on the Zanskar River in Ladakh, offering an unforgettable adventure.', 62.0,
'https://images.pexels.com/photos/1566435/pexels-photo-1566435.jpeg', 'f808ddcd-b5e5-4d80-b732-1ca523e48434', '3c333333-3333-3333-3333-333333333333');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('60666666-aaaa-bbbb-cccc-666666666666', 'Goecha La Trek', 'A high-altitude trek in Sikkim that offers spectacular views of Kanchenjunga.', 45.0,
'https://images.pexels.com/photos/1547613/pexels-photo-1547613.jpeg', 'f808ddcd-b5e5-4d80-b732-1ca523e48434', '4d444444-4444-4444-4444-444444444444');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('70777777-aaaa-bbbb-cccc-777777777777', 'Tawang Monastery Trail', 'A serene cultural trek through beautiful valleys and monasteries in Arunachal Pradesh.', 10.0,
'https://images.pexels.com/photos/1868778/pexels-photo-1868778.jpeg', '54466f17-02af-48e7-8ed3-5a4a8bfacf6f', '5e555555-5555-5555-5555-555555555555');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('80888888-aaaa-bbbb-cccc-888888888888', 'Chembra Peak Trek', 'A scenic trek to the highest peak in Wayanad, Kerala, known for its heart-shaped lake.', 7.0,
'https://images.pexels.com/photos/248062/pexels-photo-248062.jpeg', 'ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c', '6f666666-6666-6666-6666-666666666666');

INSERT [dbo].[Walks] ([Id], [Name], [Description], [LengthInKm], [WalkImageUrl], [DifficultyId], [RegionId])
VALUES
('90999999-aaaa-bbbb-cccc-999999999999', 'Dudhsagar Waterfall Trek', 'An adventurous trek along railway tracks leading to the majestic Dudhsagar Falls in Goa.', 12.0,
'https://images.pexels.com/photos/372281/pexels-photo-372281.jpeg', 'ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c', '7g777777-7777-7777-7777-777777777777');
GO
