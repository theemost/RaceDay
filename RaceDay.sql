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

create table Categories(
	categoryId int identity(6000, 1) primary key,
	categoryName varchar(100) not null,
	eventId int,
	foreign key(eventId) references Events(eventId)
);

create table Enrolments(
	enrolmentId int identity(4000, 1) primary key,
	participentId int,
	foreign key(participentId) references Users(userId),
	eventId int,
	foreign key(eventId) references Events(eventId),
	categoryId int,
	foreign key(categoryId) references Categories(categoryId),
	enrolmentStatus varchar(10) not null
);


create table Results(
	resultId int identity(1, 1) primary key,
	enrolmentId int unique,
	foreign key(enrolmentId) references Enrolments(enrolmentId),
	finishTime varchar(100),
	finishPosition int
);