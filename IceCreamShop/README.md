# Ice Cream Shop

An ASP.NET Core Web API and static web page for managing an ice-cream menu. This project follows the same controller-based ASP.NET Core structure as the teacher's Weather Forecast example: `Program.cs` registers controllers, controller classes use `[ApiController]` and attribute routes, and response data is represented by model classes. The original pizza-shop features are adapted to ice-cream flavors.

## Run the application

Requirements: the .NET 10 SDK.

From this directory:

```powershell
dotnet restore
dotnet run
```

In Development, the launch profile opens Swagger at `https://localhost:5001/swagger`. The shop page is at `https://localhost:5001/`. If the HTTPS development certificate is unavailable, use `http://localhost:5000/`.

To build without starting the server:

```powershell
dotnet build
```

## Project map

| File or folder | Purpose |
| --- | --- |
| `.gitignore` | Keeps generated .NET build output out of source control. |
| `IceCreamShop.csproj` | Web SDK project settings, target framework, and Swagger dependency. |
| `Program.cs` | Application entry point; registers MVC controllers and the menu service, then assembles the HTTP middleware pipeline. |
| `appsettings.json` | Default logging, allowed hosts, and the `ShopClosed` switch. |
| `appsettings.Development.json` | Logging overrides used in Development. |
| `Properties/launchSettings.json` | Local Development launch profile, URLs, and Swagger start page. |
| `Controllers/IceCreamController.cs` | CRUD HTTP endpoints for ice-cream flavors. |
| `Controllers/WeatherForecastController.cs` | The generated-style Weather Forecast sample endpoint retained as a teaching reference. |
| `Models/IceCream.cs` | Menu item shape: ID, required name, and dairy-free flag. |
| `Models/WeatherForecast.cs` | Weather Forecast response shape from the controller example. |
| `Interfaces/IIceCreamService.cs` | Contract between the controller and menu storage implementation. |
| `Services/IceCreamService.cs` | Loads, queries, changes, and saves the menu JSON file. |
| `Data/icecream.json` | Active menu data file read and written by `IceCreamService`. |
| `icecream.json` | Matching root-level copy retained from the source project's duplicate data-file layout; runtime changes are made only to `Data/icecream.json`. |
| `Middleware/ShopClosedMiddleware.cs` | Returns HTTP 503 early when the shop is configured as closed. |
| `wwwroot/index.html` | Static browser UI markup served at `/`. |
| `wwwroot/css/site.css` | Layout and visual styles for the menu UI. |
| `wwwroot/js/site.js` | Browser-side requests, menu rendering, and form actions. |

## Request pipeline

`Program.cs` follows the Weather Forecast / controller project shape:

1. Register MVC controllers and Swagger services.
2. Register `IIceCreamService` as a singleton implemented by `IceCreamService`.
3. In Development, enable Swagger and Swagger UI.
4. Run `ShopClosedMiddleware` before static files and controllers, so closing the shop blocks both.
5. Serve the default page and files from `wwwroot`.
6. Map controller routes.

The middleware calls the next component when `ShopClosed` is `false`. When it is `true`, it short-circuits the request with status `503 Service Unavailable` and a plain-text message. Change the value in `appsettings.json` and restart the app to change this setting.

## API

The controller route is `/IceCream` (case-insensitive in ASP.NET Core). Swagger documents the endpoints at `/swagger`.

| Method | Route | Behavior |
| --- | --- | --- |
| `GET` | `/IceCream` | Returns the full flavor list. |
| `GET` | `/IceCream/{id}` | Returns one flavor, or `404` if it does not exist. |
| `POST` | `/IceCream` | Adds a flavor and returns `201 Created` with its location and representation. |
| `PUT` | `/IceCream/{id}` | Replaces a flavor; returns `400` if body and route IDs differ, `404` if missing, otherwise `204`. |
| `DELETE` | `/IceCream/{id}` | Deletes a flavor; returns `404` if missing, otherwise `204`. |
| `GET` | `/WeatherForecast` | Returns five sample forecasts, following the teacher's controller-based Weather Forecast example. |

Example request body:

```json
{
  "name": "Pistachio",
  "isDairyFree": false
}
```

IDs are assigned by the service. Updates require the ID in the JSON body to match the ID in the URL. The `Name` field is required and is validated by `[ApiController]`.

## Menu storage

The service reads `Data/icecream.json` when it is first created and saves every successful add, update, and delete back to that file. This is intentionally a small teaching example, not a database: it has no multi-process locking or transaction support. The active data file must remain present and contain a valid JSON array.

To restore the starter menu, copy the contents of the root-level `icecream.json` into `Data/icecream.json`.

## Browser UI

Open `/` to view the menu. The page uses the `/IceCream` API to load flavors and supports adding, editing, and deleting them. Each flavor has a dairy-free checkbox. API failures are displayed on the page rather than silently ignored.

The JavaScript creates menu names as text nodes instead of interpreting them as HTML. The page is served by ASP.NET Core static-file middleware and does not require a separate frontend build.

## Test the project manually

1. Start the app and open `/swagger`.
2. Call `GET /IceCream` and confirm the starter flavors appear.
3. Use `POST /IceCream` to add a flavor, then verify it appears in the browser page and in `Data/icecream.json`.
4. Update it with `PUT /IceCream/{id}`, then remove it with `DELETE /IceCream/{id}`.
5. Set `ShopClosed` to `true`, restart, and confirm API and page requests return `503`; set it back to `false` afterward.
6. Call `GET /WeatherForecast` to compare with the retained Weather Forecast sample controller.
