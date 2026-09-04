create database RaceDay;

use RaceDay;

create table Users(
	userId int identity(1000, 1) primary key,
	firstName varchar(50) not null,
	lastName varchar(50) not null,
	emailAddress varchar(100) not null,
	password varchar(100),
	username varchar(50),
	role varchar(50)
);

create table EventTypes(
	eventTypeId int identity(5000, 1) primary key,
	eventTypeName varchar(50) not null
);

create table Events(
	eventId int identity(2000, 1) primary key,
	eventName varchar(50) not null,
	description varchar(100) not null,
	eventDate varchar(11) not null,
	location varchar(100) not null,
	organiserId int,
	foreign key(organiserId) references Users(userId),
	eventTypeId int,
	foreign key(eventTypeId) references EventTypes(eventTypeId)
);
