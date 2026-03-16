# EventEase – Part 1 Deployment

## Project Overview

EventEase is a web-based event management system developed for the POE assignment. The purpose of the system is to allow users to manage events, venues, and bookings in a structured way.

For **Part 1**, the focus of the project was to:

* Design and implement the database
* Develop the basic web application
* Implement user authentication
* Deploy the web application and database to Microsoft Azure

The system was developed using **ASP.NET Core MVC** and **SQL Server**, and deployed to the cloud using **Azure App Service** and **Azure SQL Database**.

---

## System Features Implemented (Part 1)

### User Authentication

The system includes a login and sign-up system that allows users to access the application securely. Passwords are stored using password hashing to improve security.

Two roles are supported in the system:

* **Admin**
* **Booking Specialist**

A default administrator account is automatically created when the application runs for the first time.

---

### Event Management

The system allows events to be created and managed. Each event includes the following information:

* Event name
* Event date
* Event description
* Event image

---

### Venue Management

Venues can be added and managed within the system. Each venue contains:

* Venue name
* Location
* Capacity
* Venue image

---

### Booking Management

The booking feature links events and venues together. This allows the system to record which event is booked at which venue.

Each booking stores:

* The event
* The venue
* The booking date

---

## Database Design

The system uses a relational database with three main tables.

### Venue Table

| Field     | Description                  |
| --------- | ---------------------------- |
| VenueID   | Primary key                  |
| VenueName | Name of the venue            |
| Location  | Venue location               |
| Capacity  | Maximum number of attendees  |
| ImageUrl  | Image representing the venue |

### Event Table

| Field       | Description       |
| ----------- | ----------------- |
| EventID     | Primary key       |
| EventName   | Name of the event |
| EventDate   | Date of the event |
| Description | Event description |
| ImageUrl    | Event image       |

### Booking Table

| Field       | Description                   |
| ----------- | ----------------------------- |
| BookingID   | Primary key                   |
| EventID     | Foreign key referencing Event |
| VenueID     | Foreign key referencing Venue |
| BookingDate | Date of the booking           |

The **Booking table** acts as the relationship between events and venues.

---

## Technologies Used

The following technologies were used to build the system:

* **C#**
* **ASP.NET Core MVC**
* **Entity Framework Core**
* **SQL Server**
* **HTML and CSS**
* **Bootstrap**

---

## Cloud Deployment

For Part 1 of the project, the application was deployed to the cloud using Microsoft Azure.

### Azure App Service

The web application was deployed using **Azure App Service**, which allows the application to be hosted and accessed through a web browser.

### Azure SQL Database

The database was migrated from a local SQL Server instance to **Azure SQL Database** so that the application can access data from the cloud.

The application connects to the database using a connection string stored in the `appsettings.json` file.

---

## Default Admin Login

The system seeds a default admin account when the application starts.

**Email:**
[admin@eventease.com](mailto:admin@eventease.com)

**Password:**
Admin123

This account allows access to the administrator dashboard.

---

## Running the Application

To run the application locally:

1. Open the project in Visual Studio.
2. Ensure the connection string is configured correctly.
3. Run the application.
4. Log in using the default admin credentials.

---

## Conclusion

Part 1 of the EventEase project focused on building the core functionality of the system, including database design, authentication, and cloud deployment. The application was successfully deployed to Microsoft Azure with both the web application and database running in the cloud.
