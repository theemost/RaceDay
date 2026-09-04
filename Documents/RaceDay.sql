create database RaceDay;

use RaceDay;

create table Users(
	userId int identity(1000, 1) primary key,
	firstName varchar(50) not null,
	lastName varchar(50) not null,
	emailAddress varchar(100) not null unique,
	password varchar(100) not null,
	username varchar(50) not null unique,
	role varchar(50) not null
);

create table EventTypes(
	eventTypeId int identity(5000, 1) primary key,
	eventTypeName varchar(50) not null
);

create table Events(
	eventId int identity(2000, 1) primary key,
	eventName varchar(50) not null,
	description varchar(100) not null,
	eventDate date not null,
	location varchar(100) not null,
	organiserId int not null,
	foreign key(organiserId) references Users(userId),
	eventTypeId int not null,
	foreign key(eventTypeId) references EventTypes(eventTypeId)
);

create table Categories(
	categoryId int identity(6000, 1) primary key,
	categoryName varchar(100) not null,
	eventId int not null,
	foreign key(eventId) references Events(eventId)
);

create table Enrolments(
	enrolmentId int identity(4000, 1) primary key,
	participantId int not null,
	foreign key(participantId) references Users(userId),
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
	finishTime time not null,
	finishPosition int not null
);

insert into Users(firstName, lastName, username, emailAddress, password, role)
values	('Esethu', 'Busakwe', 'esethu@gmail.com', 'HASHED_PASSWORD1', 'esethub', 'Organiser'), 
		('John','Mokoena','john@gmail.com','HASHED_PASSWORD2','johnm','Organiser'),
		('Walter','White','walterw@gmail.com','HASHED_PASSWORD3','wwhite','Participant'),
		('Marty','Byrd','martybyrd@gmail.com','HASHED_PASSWORD4','byrdm','Participant');

insert into EventTypes(eventTypeName)
values	('Run'), ('Swimming'), ('Cycling'), ('Walk');

insert into Events(eventName, description, eventDate, location, organiserId, eventTypeId)
values	('Run Jozi', '100km marathon', '2026-10-09', 'Ghandi Square', 1000, 5000),
		('Swim Meet', '10Km swim', '2026-9-10', 'Richards Bay', 1001, 5001),
		('Tour de Jozi', '150km race', '2026-10-20', 'Braamfontein', 1000, 5002)


