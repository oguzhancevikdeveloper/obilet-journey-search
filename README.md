# Obilet Journey Search

An ASP.NET Core MVC application for searching bus journeys using the Obilet API.

## Features

- Search bus journeys by origin, destination, and departure date.
- Retrieve bus locations from the Obilet API.
- Search locations using autocomplete.
- Swap origin and destination.
- Select today or tomorrow as the departure date.
- Validate that origin and destination are different.
- Prevent searches for past departure dates.
- Display journeys sorted by departure time.
- Store the API session per user.
- Keep API credentials on the server side.

## Technologies

- C#
- ASP.NET Core MVC
- .NET 10
- Razor Views
- JavaScript
- CSS
- Obilet API
- xUnit

## Requirements

- .NET 10 SDK
- A valid Obilet API client token

## Configuration

Create a `.env` file in the project root directory and configure the required API settings:

```env
OBILET_API_BASE_URL=https://v2-api.obilet.com/api/
OBILET_API_CLIENT_TOKEN=YOUR_API_CLIENT_TOKEN
```

Replace the placeholder values with the credentials and configuration values provided for your environment.

**Important:** Never commit your real API token or other sensitive credentials to version control. The `.env` file should remain excluded from Git.

## Run the Application

Restore dependencies:

```bash
dotnet restore
```

Run the application:

```bash
dotnet run --project src/Obilet.JourneySearch.Web
```

Open the local URL displayed in the terminal.

## Run Tests

Run the test suite from the repository root:

```bash
dotnet test
```

## Validation

The application validates search requests on both the client and server sides.

- Origin and destination must be different.
- The departure date cannot be in the past.
- Invalid requests must not proceed to journey search.

## Project Structure

```text
src/
  Obilet.JourneySearch.Web/
    Controllers/
    Models/
    Services/
    Views/
    wwwroot/

tests/
  Obilet.JourneySearch.Tests/
```

- **Controllers:** Handle incoming requests and application flow.
- **Models:** Contain API models and view models.
- **Services:** Handle Obilet API communication, session management, and journey/location operations.
- **Views:** Render the user interface using Razor.
- **wwwroot:** Contains CSS and JavaScript assets.
- **Tests:** Contains automated tests.

## API Integration

The application communicates with the Obilet API through server-side HTTP requests. API credentials are configured on the server and are not exposed to the browser.

The application uses the following API endpoints:

- `POST /client/getsession`
- `POST /location/getbuslocations`
- `POST /journey/getbusjourneys`

<img width="1255" height="795" alt="image" src="https://github.com/user-attachments/assets/b917959c-198b-4d9e-8422-154eb9632d70" />
<img width="1302" height="813" alt="image" src="https://github.com/user-attachments/assets/78decafb-4124-4705-97bf-067547f46e2a" />


## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

