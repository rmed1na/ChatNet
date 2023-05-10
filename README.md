# ChatNet
ChatNet is an ASP.NET Core MVC **chat application** that allows users to log in and chat in as many chatrooms as they want, discussing several topics about finance and economics in general. The chatrooms have bots in them with the ability of delivering stock prices anytime a user requests one.

## Requirements
- .NET 7 SDK (find binaries [here](https://dotnet.microsoft.com/en-us/download/dotnet/7.0))
- Visual Studio 2022
- Sql Server
- Erlang installation (find binaries [here](https://www.erlang.org/downloads))
- RabbitMQ installation (find binaries [here](https://www.rabbitmq.com/download.html))
- An active internet connection during runtime

## Setup
1. Clone this repository to your local environment
2. Inside the `ChatNet` solution, look for the project `ChatNet.Database`, Scripts folder, `initial.sql` file and run it's content on your Sql Server database server (admin user must be used to run this script). This script will create the following:
    2.1. The database (named `ChatNet`) with it's schema.
    2.1. The default application database login user (`cn_usr`).
    2.2. One default application user (`admin`) with a default password (`Password@123`).
    2.3. One default chatroom (`General`).

3. Inside the "ChatNet" solution, look for the project `ChatNet`, specifically for it's `appsettings.Development.json` file (or `appsettings.json` file if not run on debug mode) and put in the missing values for database and message broker credentials. Once finished the file should look like something like this:
```{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Application": {
    "DataSource": {
      "Server": "localhost",
      "Database": "ChatNet",
      "User": "cn_usr",
      "Password": "123456789"
    },
    "MessageBroker": {
      "Server": "localhost",
      "User": "guest",
      "Password": "guest"
    }
  }
}
```

> You can leave the file as the previous example if you didn't change anything on the `initial.sql` script and left the RabbitMQ credentials as they came out of the box. If not, please replace the users & passwords with your own.

4. Do the same as step 3 but for the project `ChatNet.Service` inside the solution. Once finished that file should look something like this:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Application": {
    "MessageBroker": {
      "Server": "localhost",
      "User": "guest",
      "Password": "guest"
    },
    "StockApiUrl": "https://stooq.com/q/l/?s=[ticker]&f=sd2t2ohlcv&h&e=csv"
  }
}
```
> You can keep the file as this previous example if you didn't change the RabbitMQ configuration that comes out of the box with the installation. **Please leave the StockApiUrl parameter as it is on the repo at the moment**

5. Make sure the startup order is correctly placed. Right-click on the solution name (`ChatNet`) -> Configure startup projects and make sure that "Multiple startup projects" is selected with the "Start" action on both `ChatNet` and `ChatNet.Service` projects. The rest should stay with the "None" action.

6. Rebuild the application (Click on Build -> Rebuild solution).

7. Run the application (Click F5 or the green **Start** button). You now should be prompted to the login page in the browser.

8. Initially, when you ran the **initial.sql** script a default admin user was made on the database (creds: `admin`, pass: `Password@123`) which you can use to log in and start using the application or you can use the "Register" button to create a new user.

## Basic usage
Once you log in you can go to `Chatrooms` from the top bar and see a list of the current active chatrooms. By default, the `General` chatroom is made on the `initial.sql` script execution but more chatrooms can be added with the `Add new` link on the rop right corner of the header.

To get inside a chatroom just click the name from the list and you will be redirected inside the chatroom from where you will be able to read all the conversations that happened previously and the one that's currently going on.

![Screenshot 2023-05-09 193801](https://github.com/rmed1na/ChatNet/assets/30838319/404f57d6-e09c-4060-81d1-5c536a19f098)

## Chatbot
The chatrooms have a chatbot that allows any user to query a stock quote. In order to use the bot (aka. `StockBot`) the following command is used:

```
/stock=aapl.us
```

In which, `/stock` is the command itself and `aapl.us` would be the ticker symbol of the stock to request.
![Screenshot 2023-05-09 194110](https://github.com/rmed1na/ChatNet/assets/30838319/1764f02c-3969-4097-91b1-9ff9950c058e)
