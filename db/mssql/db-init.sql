USE master
GO

IF DB_ID('Players') IS NOT NULL
  set noexec on

CREATE DATABASE Players;
GO

USE Players
GO

CREATE TABLE PlayerGame (PlayerId varchar(100), GameId int)
GO

CREATE TABLE PlayerTime (PlayerId varchar(100), StartTime int, EndTime int)
GO

