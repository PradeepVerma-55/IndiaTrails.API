
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
