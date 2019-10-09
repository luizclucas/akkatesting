# Akka Testing
- It's a project to show how akka works and how better and easy it is to write correct concurrent and parallel systems. It's using a part of DDD too.

- It's a comparsing between using a simple thread to insert things on database(Considering a delay of 200ms for each "process" that need to be done) and using actor to do that, beggning with 2 actors and stop when it's bigger than 31, incrementing 5 per 5.

### Getting Started
- In class DataFactory there is an connection: _mysqlCn, so if you want to use your own MySqlDatabase you'll need to change that connection and create a table:
  Name: Client | Fields: Id(PK, char(36), Name(varchar(150)), CPF(varchar(20)), City(varchar(100)).
  
- As you don't need to change mysql connection, everything you need is to run AkkaTesting under 0 - Presentation Folder, you can run it both in Visual Studio or running on powershell these commands: 1. dotnet build 2. dotnet publish 3. dotnet run {dllname}


### Prerequisites
- Dotnetcore 3.0 => you can get it on: https://dotnet.microsoft.com/download
- MysqlDatabase => if you want your personal one, so you'll need to create a table named Client(Id,Name,CPF,City)


### Give examples
- On program, there's a Run, there's some variables listed bellow, that you can change the number of insert's that will be done.    
      public static int _totalInsert = 50;
      public static int _timesToRun = 5;
      
- On Luiz : ReceiveActor there's int _workerMax = 31, that is used to stop testing with more and more actors.
- On Luiz: Receive<EndActor> there's a incriment of number of actors each time the code pass there.  


### Authors
Luiz Claudio Dias Lucas - Initial work - [Luiz Lucas](https://github.com/luizclucas)
### License
You can use it as you wish.

### Acknowledgments
You can check the whole akka documentation at: https://getakka.net/index.html
