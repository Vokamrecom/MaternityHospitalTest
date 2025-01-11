# MaternityHospitalAPI

## Description

This project provides an API to manage patient data. It includes all necessary components, such as SQL Server and the API server, which are deployed using Docker. Additionally, there is a console application that adds 100 patients to the database via the API. A Postman collection for testing the API is also included in the archive.

The database will be automatically set up with the required tables when the containers are started.

Task description https://drive.google.com/file/d/1NB9T5awDSK_Bygole8jcD9NfIBupZlCc/view .

## Requirements

Docker must be installed on your machine to run this project.

## Running the Project

1. Clone the repository:

   ```bash
   git clone <repository_url>

2. Navigate to the project directory:
   ```bash
   cd <project_directory>

3. Set up the project and SQL Server by running the following command:
   ```bash
   docker-compose up --build

This will create and start the containers for SQL Server and the API project.

4. To populate the database with 100 patients, run the patient_api_client container:
   ```bash
   docker-compose run --rm patient_api_client
   
# Postman Collection
A Postman collection (in .json format) is included in the archive for testing the API.

Import the collection file(Maternity Hospital API Demo.postman_collection.json) into Postman.
Execute the requests as per the documentation.
