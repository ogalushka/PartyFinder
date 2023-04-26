USE master
GO

IF DB_ID('Players') IS NOT NULL
  set noexec on

CREATE DATABASE Players;
GO

USE Players
GO

CREATE TABLE Games (
	GameId int NOT NULL,
	Name varchar(100) NOT NULL,
	CoverUrl varchar(150),
	PRIMARY KEY (GameId)
)
GO

CREATE TABLE PlayerGame (
	PlayerId varchar(100) NOT NULL,
	GameId int NOT NULL
	PRIMARY KEY (PlayerId, GameId),
	FOREIGN KEY (GameId) REFERENCES Games(GameId)
)
GO

CREATE TABLE PlayerTime (
	PlayerId varchar(100) NOT NULL,
	StartTime int NOT NULL,
	EndTime int NOT NULL
)
GO




