## Directory for server part of an app

### Functionality:
 - receiving data from the Frontend Web App
 - sending data(photo) to the model
 - receiving data from model(text from image)
 - checking received text for validity
 - perform mathematical operations from received text, if it's valid
 - send answer for math problem to the Frontend

### How to start Backend:
1. Install .NET
2. dotnet add package Microsoft.Azure.CognitiveServices.Vision.ComputerVision
3. set environment variables VISION_KEY and VISION_ENDPOINT
(example, in PowerShell: $env:VISION_ENDPOINT = "your endpoint", same for VISION_KEY)
4. In the `appsettings.json` file, replace the value of the `"DefaultConnection"` field with your **connection string** obtained from the database service Azure SQL Database
5. dotnet run
